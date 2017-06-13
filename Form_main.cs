using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Collections;
using System.Net;
using MediaInfoLib;
using System.Globalization;

namespace avidInSchedule
{
    public partial class Form_main : Form
    {
        public Form_main()
        {
            InitializeComponent();

            logpath = Application.StartupPath + "\\logs";
            
            if (!Directory.Exists(logpath))
            {
                Directory.CreateDirectory(logpath);
            }

            if (!Directory.Exists(Application.StartupPath + "\\avidingestxml"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\avidingestxml");
            }

            if (!Directory.Exists(Application.StartupPath + "\\videoxml"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\videoxml");
            }

            if (!Directory.Exists(Application.StartupPath + "\\timeLenFile"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\timeLenFile");
            }

            //生成count文件
            if (!Directory.Exists(Application.StartupPath + "\\counts"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\counts");
            }


            if (!Directory.Exists(Application.StartupPath + "\\mediainfo"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\mediainfo");
            }

            mediainfoLists = new List<MediaInfos>();
            htrels = new Hashtable();
            WriteLogNew.writeLog("软件启动!",logpath,"info");
            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"软件启动!\n");

            timer_check.Enabled = false;
           
            //读取xml 
            #region 读取avidin xml
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "\\avidinconfig.xml");
            System.Xml.XmlElement root = doc.DocumentElement;
            XmlNodeList avidinserverlist = root.SelectNodes("//avidinserver"); //avidin serverlist
            for (int i = 0; i < avidinserverlist.Count; i++)
            {
                XmlNode avidinserverNode = avidinserverlist.Item(i);
                XmlNode ifworksNode = avidinserverNode.FirstChild;
                if (ifworksNode.InnerText.Equals("0"))
                {
                    continue;
                }
                else
                {
                    XmlNode servernameNode = ifworksNode.NextSibling;
                    string servername = servernameNode.InnerText;
                    
                    XmlNode serverIPNode = servernameNode.NextSibling;
                    string serverIP = serverIPNode.InnerText;

                    XmlNode filePathNode = serverIPNode.NextSibling;
                    string filePath = filePathNode.InnerText;

                    XmlNode iftakeLongTaskNode = filePathNode.NextSibling;
                    int iftakeLongTask = Convert.ToInt32(iftakeLongTaskNode.InnerText);
                  
                    this.dataSetMain.avidin.Rows.Add(servername, serverIP, filePath, 0, "离线", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "", iftakeLongTask,"00:00:00:00");
                            
                }  //else
            } // for(int i = 0 ;i < avidinserverlist.Count;i++)


            XmlNodeList mediaListNodes = root.SelectNodes("//mediaFiles");
            for (int i = 0; i < mediaListNodes.Count; i++)
            {
                MediaInfos mis = new MediaInfos();
                XmlNode mediaNode = mediaListNodes.Item(i);
                XmlNode ifworksNode = mediaNode.FirstChild;
                if (ifworksNode.InnerText.Equals("0"))
                {
                    continue;
                }
                else
                {
                    XmlNode srcFromNode = ifworksNode.NextSibling;
                    mis.SrcFrom = srcFromNode.InnerText;

                    XmlNode sPathNodexml = srcFromNode.NextSibling;
                    mis.SPathxml = sPathNodexml.InnerText;

                    XmlNode sPathNodevideo = sPathNodexml.NextSibling;
                    mis.SPathvideo = sPathNodevideo.InnerText;

                    XmlNode ifcontailsNextFoldersNode = sPathNodevideo.NextSibling;
                    mis.IfcontailsNextFolders= Convert.ToInt32(ifcontailsNextFoldersNode.InnerText);

                    XmlNode transcodeFilePathNode = ifcontailsNextFoldersNode.NextSibling;
                    mis.TranscodeFilePath = transcodeFilePathNode.InnerText;

                    mediainfoLists.Add(mis);
                }
            }// for (int i = 0; i < scriptinfoList.Count; i++)

            #endregion
           
            //读取Relation xml 
            readRelationXml();

            longTaskTime = ConvertFrame(Properties.Settings.Default.longTaskTime, 0) * 40; //ms

            mediaxml = new MediaInfoXmlClass();

            WriteLogNew.writeLog("读取avidinserver 配置信息成功！", logpath, "info");
            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"读取avidinserver 配置信息成功！\n");

            timer_check.Interval = Properties.Settings.Default.checkStatusInterval;

            timer_check.Enabled = true;

         
            videoThread = new Thread(new ThreadStart(scanVideoThread));
            videoThread.IsBackground = true;
            videoThread.Start();

        }

        #region 变量定义
        private MediaInfoXmlClass mediaxml = null;
        private string logpath;
        private List<MediaInfos> mediainfoLists;
        private Hashtable htrels;
        private Thread videoThread;
        private int longTaskTime= 1200*25 ; //20分钟
        private List<string> mobilephones;
        #endregion

        #region 消息框代理
        private delegate void SetTextCallback(string text);
        private delegate void SetSelectCallback(object Msge);
        private void SetText(string text1)
        {
            string text =  text1;
            try
            {
                if (this.richTextBox1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    this.Invoke(d, new object[] { text });
                }
                else
                {
                    this.richTextBox1.AppendText(text);
                    of_SetRichCursor(richTextBox1);
                }
            }
            catch (Exception)
            {
            }
        }
        private void of_SetRichCursor(object msge)
        {
            try
            {
                RichTextBox richbox = (RichTextBox)msge;
                //设置光标的位置到文本尾
                if (richbox.InvokeRequired)
                {
                    SetSelectCallback d = new SetSelectCallback(of_SetRichCursor);
                    this.Invoke(d, new object[] { msge });
                }
                else
                {
                    richbox.Select(richbox.TextLength, 0);
                    //滚动到控件光标处
                    richbox.ScrollToCaret();
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        /// <summary>
        /// 将总帧数转换为标准时间格式
        /// </summary>
        /// <param name="frame_num">总帧数</param>
        /// <returns></returns>
        private string ConvertTime(int frame_num)
        {
            int frame = frame_num % 25;//获取总帧数
            string frame_str = frame.ToString();
            if (frame >= 10)
            {
                frame_str = frame_str.ToString();
            }
            else
            {
                frame_str = "0" + frame_str.ToString();
            }

            int frame_remain = frame_num / 25;//除去帧数，得到剩余总帧数
            int second = frame_remain % 60;//获取总秒数
            string second_str = second.ToString();
            if (second >= 10)
            {
                second_str = second_str.ToString();
            }
            else
            {
                second_str = "0" + second_str.ToString();
            }

            int second_remain = frame_remain / 60;//除去帧数和秒数，得到剩余总秒数
            int minute = second_remain % 60;//获得总分钟数
            string minute_str = minute.ToString();
            if (minute >= 10)
            {
                minute_str = minute_str.ToString();
            }
            else
            {
                minute_str = "0" + minute_str.ToString();
            }

            int minute_remain = second_remain / 60;//除去帧数，秒数和分钟数，得到剩余总分钟数
            int hour = minute_remain % 60;//得到总小时数
            string hour_str = hour.ToString();
            if (hour >= 10)
            {
                hour_str = hour_str.ToString();
            }
            else
            {
                hour_str = "0" + hour_str.ToString();
            }

            string time = hour_str + ":" + minute_str + ":" + second_str + ":" + frame_str;
            //string time = hour_str + ":" + minute_str + ":" + second_str + ":00";
            return time;

        }
        /// <summary>
        /// 将标准时间格式转化为帧数
        /// </summary>
        /// <param name="time">表示时间的字符串</param>
        /// <returns></returns>
        private int ConvertFrame(string time, int pal)
        {
            int time_all = 0;
            if (pal == 0)  //pal
            {
                int time_h = Convert.ToInt32(time.Split(':')[0]) * 3600 * 25;
                int time_m = Convert.ToInt32(time.Split(':')[1]) * 60 * 25;
                int time_s = Convert.ToInt32(time.Split(':')[2]) * 25;
                int time_f = Convert.ToInt32(time.Split(':')[3]);
                time_all = time_h + time_m + time_s + time_f;
            }
            else //ntsc
            {
                int time_h = Convert.ToInt32(time.Split(';')[0]) * 3600 * 30;
                int time_m = Convert.ToInt32(time.Split(';')[1]) * 60 * 30;
                int time_s = Convert.ToInt32(time.Split(';')[2]) * 30;
                int time_f = Convert.ToInt32(time.Split(';')[3]);
                time_all = time_h + time_m + time_s + time_f;
            }
            return time_all;
        }

        private void readRelationXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Application.StartupPath + "\\relation.xml");
            System.Xml.XmlElement root = doc.DocumentElement;
            XmlNodeList relationlist = root.SelectNodes("//relation"); //relation serverlist
            foreach (XmlNode relnode in relationlist)
            {
                XmlNode keynode = relnode.FirstChild;
                XmlNode valuenode = keynode.NextSibling;
                htrels.Add(keynode.InnerText, valuenode.InnerText);
            }
        }
       
        private void scanVideoThread()
        {
            Thread.Sleep(10000);
            while (true)
            {
                foreach (MediaInfos mi in mediainfoLists)//可能有多个系统对应的配置文件信息
                {
                    List<string> mediamd5lists = new List<string>(); //根据是否生成md5文件来判断文件是否转码完成

                    string[] md5s = Directory.GetFiles(mi.TranscodeFilePath, "*.md5sum", SearchOption.TopDirectoryOnly);
                    mediamd5lists = md5s.ToList<string>();

                    foreach (string mediamd5 in mediamd5lists)
                    {
                        //判断文件是否处理过 
                        string newlocalmd5file = Application.StartupPath + "\\videoxml" + "\\" + Path.GetFileName(mediamd5);

                        if (File.Exists(newlocalmd5file))
                        {
                            continue;
                        }
                        //该素材没有处理过 
                        WriteLogNew.writeLog("开始处理video 调度！" +  Path.GetFileName( newlocalmd5file ), logpath, "info");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "开始处理video 调度！" + Path.GetFileName( newlocalmd5file )+"\n");

                        string newcountfile = Application.StartupPath + "\\counts\\" + Path.GetFileNameWithoutExtension(newlocalmd5file) + "count.xml";
                        try
                        {
                            if (!File.Exists(newcountfile))
                            {
                                File.Copy(Application.StartupPath + "\\counts.xml", newcountfile,true);
                            }

                            XmlDocument doccount = new XmlDocument();
                            doccount.Load(newcountfile);
                            System.Xml.XmlElement rootcount = doccount.DocumentElement;

                            XmlNode countnode = rootcount.SelectSingleNode("/root/counts");
                            string nowcount = countnode.InnerText;
                            int newcount = Convert.ToInt32(nowcount) + 1;
                            countnode.InnerText = newcount.ToString();
                            doccount.Save(newcountfile);

                            WriteLogNew.writeLog("生成count xml！", logpath, "info");
                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "生成count xml！\n");

                            if (newcount > Properties.Settings.Default.errorRetryCounts)
                            {
                                WriteLogNew.writeLog("重试次数大于系统设定值，将该节目设置成error！", logpath, "error");
                                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "重试次数大于系统设定值，将该节目设置成error！\n");
                                //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "重试次数大于系统设定值，将该节目设置成error!" + Path.GetFileName(xmlfile) + "\n");

                                //将文件设置成已处理
                                File.Copy(mediamd5, newlocalmd5file, true);
                                WriteLogNew.writeLog("将md5设置成已处理状态!" + Path.GetFileName( mediamd5), logpath, "info");
                                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "将md5设置成已处理状态!" +Path.GetFileName(  mediamd5) + "\n");

                                //try
                                //{
                                //    File.Delete(mediamd5);
                                //    WriteLogNew.writeLog("将MD5文件删除:" + Path.GetFileName(mediamd5), logpath, "info");
                                //}
                                //catch (Exception ee)
                                //{
                                //    WriteLogNew.writeLog("将MD5文件删除异常:" + Path.GetFileName(mediamd5) + ee.ToString(), logpath, "info");
                                //    Thread.Sleep(1000);
                                //    try
                                //    {
                                //        File.Delete(mediamd5);
                                //        WriteLogNew.writeLog("将MD5文件删除:" + Path.GetFileName(mediamd5), logpath, "info");
                                //    }
                                //    catch (Exception eet)
                                //    {
                                //        WriteLogNew.writeLog("将MD5文件删除异常:" + Path.GetFileName(mediamd5) + eet.ToString(), logpath, "info");
                                //    }
                                //}//第一次删除异常

                                filedelteTimes(mediamd5);
                                continue;
                            } //超过重试次数
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("生成count文件统计失败!" + ee.ToString(), logpath, "error");
                            SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "生成count文件统计失败!\n");
                            //SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + "生成文件统计失败!" + "\n");
                            continue;
                        }

                        TranscodeFileInfos transcodefileinfo = new TranscodeFileInfos();
                        transcodefileinfo.SPathxml = mediamd5;

                        //如果是xnews的xml 那么不需要下发到转码 只需要生成avidingest的xml即可
                        if (mi.SrcFrom.ToLower().Equals("xnews"))
                        {
                          
                            transcodefileinfo.Newavidfilename = Path.GetFileNameWithoutExtension(mediamd5) ;
                            transcodefileinfo.TranscodeFilePath = mi.TranscodeFilePath;
                            //生成avid ingest的xml
                            string newavidingestxml = Application.StartupPath + "\\avidingestxml\\" + Path.GetFileNameWithoutExtension(mediamd5)+".xml";
                            File.Copy(Application.StartupPath + "\\avidin.xml", newavidingestxml, true);

                            XmlDocument docavidinxml = new XmlDocument();
                            docavidinxml.Load(newavidingestxml);
                            XmlNode rootavidin = docavidinxml.DocumentElement;
                            XmlNode sourceSystemNode = rootavidin.SelectSingleNode("//sourceSystem");
                            sourceSystemNode.InnerText = mi.SrcFrom;

                            XmlNode fileNameNode = rootavidin.SelectSingleNode("//fileName");
                            fileNameNode.InnerText = transcodefileinfo.Newavidfilename+".mxf";

                            //filePath
                            XmlNode filePathNodeavid = rootavidin.SelectSingleNode("//filePath");
                            string mxffilepath = transcodefileinfo.TranscodeFilePath + "\\" + transcodefileinfo.Newavidfilename + ".mxf";
                            string newmxffilepath = transcodefileinfo.TranscodeFilePath + "\\N" + DateTime.Now.ToString("yyyyMMddHHmmssfff")+".mxf";
                            //  //先把视频文件改名成 NyyyyMMddHHmmssfff.mxf
                            try
                            {
                                File.Move(mxffilepath, newmxffilepath);
                                WriteLogNew.writeLog("将原素材修改名称:"+newmxffilepath,logpath,"info");
                                WriteLogNew.writeLog("原素材名称:" +Path.GetFileName( mxffilepath) + " 新名称:" +Path.GetFileName( newmxffilepath), logpath, "info");

                                filePathNodeavid.InnerText = newmxffilepath;

                                bool iflongtasksend = false; //默认为短视频文件

                                XmlNode durationNode = rootavidin.SelectSingleNode("//duration"); //<duration>00:02:00:00</duration>
                                try
                                {
                                    //调用mediainfo 获取素材长度
                                    string xmlmedia = mediaxml.of_GetXmlStr(newmxffilepath);
                                    if (xmlmedia.Equals("Not Media File"))
                                    {
                                        WriteLogNew.writeLog("获取素材媒体mediainfo信息失败！" + newmxffilepath + "Not Media File!", logpath, "error");
                                    }
                                    else
                                    {
                                        WriteLogNew.writeLog("获取素材媒体mediainfo信息成功！" + newmxffilepath, logpath, "info");

                                        XmlDocument docmediainfo = new XmlDocument();
                                        docmediainfo.LoadXml(xmlmedia);
                                        XmlNode xmlnoderesult = docmediainfo.SelectSingleNode("//item[@Name='Duration']");
                                        int sdur = Convert.ToInt32(xmlnoderesult.InnerText);
                                        if (sdur > longTaskTime)  //超过设定的最大时间 
                                        {
                                            iflongtasksend = true;
                                            WriteLogNew.writeLog("该任务为长任务！",logpath,"info");
                                        }
                                        string tcdur = ConvertTime(sdur / 40);
                                        durationNode.InnerText = tcdur;
                                        WriteLogNew.writeLog("获取素材媒体！" + newmxffilepath + "时长:" + tcdur, logpath, "info");
                                        string mediainfoxml = Application.StartupPath + "\\mediainfo\\" + Path.GetFileNameWithoutExtension(newmxffilepath) + ".xml";
                                        docmediainfo.Save(mediainfoxml);
                                    }
                                }
                                catch (Exception ee)
                                {
                                    WriteLogNew.writeLog("获取素材媒体mediainfo信息失败！" + newmxffilepath + " " + ee.ToString(), logpath, "error");
                                }

                                //interplay 路径
                                XmlNode avidInterplayPathNode = rootavidin.SelectSingleNode("//avidInterplayPath");

                                string xnewsmediaxml = mi.SPathxml + "\\" + transcodefileinfo.Newavidfilename + ".xml";

                                string interplayPath = getInterplayWithXnews(xnewsmediaxml);

                                if (!string.IsNullOrEmpty(interplayPath))
                                {
                                    avidInterplayPathNode.InnerText = interplayPath;
                                }

                                XmlNode avidInterplayFileNameNode = rootavidin.SelectSingleNode("//avidInterplayFileName");

                                avidInterplayFileNameNode.InnerText = transcodefileinfo.Newavidfilename + ".mxf";

                                docavidinxml.Save(newavidingestxml);
                                
                                WriteLogNew.writeLog("生成avid ingest xml!" + newavidingestxml, logpath, "info");
                                
                                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "生成avid ingest xml!" + Path.GetFileName( newavidingestxml)+"\n");

                                //判断调度给那个avidingest程序 
                                //判断任务的时长 确认可以下发的avidingest的数量
                                List<DataSetMain.avidinRow> ingestrowList = new List<DataSetMain.avidinRow>();
                                foreach (DataSetMain.avidinRow row in this.dataSetMain.avidin.Rows)
                                {
                                    if (iflongtasksend)  //是否为长时间的任务
                                    {
                                        if (row.ColIfTakeLongTask == 1)
                                        {
                                            if (row.ColStatus.Equals("在线"))
                                            {
                                                ingestrowList.Add(row);
                                                WriteLogNew.writeLog("找到可用的server :" + row.ColAvidinName+"IP:"+row.ColIP, logpath, "info");
                                            }
                                        }
                                    }
                                    else //短时间任务
                                    {
                                        if (row.ColStatus.Equals("在线"))
                                        {
                                            ingestrowList.Add(row);
                                            WriteLogNew.writeLog("找到可用的server :" + row.ColAvidinName + "IP:" + row.ColIP, logpath, "info");
                                        }
                                    }
                                }
                                WriteLogNew.writeLog("找到可用的server 数量:"+ingestrowList.Count.ToString(),logpath,"info");

                                DataSetMain.avidinRow rowfind = null;
                                if (ingestrowList.Count > 0)
                                {
                                    rowfind = ingestrowList[0];

                                    int taskcount = rowfind.ColTaskCount;

                                    if (taskcount == 0)  //第一个 avidingest 就没有任务
                                    {
                                        WriteLogNew.writeLog("找到server :" + rowfind.ColAvidinName + "IP:" + rowfind.ColIP + " 当前任务数:" + rowfind.ColTaskCount.ToString(), logpath, "info");
                                    }
                                    else
                                    {
                                        int tasktimeLenFrame = 0;
                                        try
                                        {
                                            tasktimeLenFrame = ConvertFrame(rowfind.ColTaskTimeLen, 0);
                                        }
                                        catch (Exception ee)
                                        {
                                            WriteLogNew.writeLog("获取最佳avid ingest server  时长异常！" + ee.ToString(), logpath, "error");
                                        }

                                        int kk = 0;
                                        foreach (DataSetMain.avidinRow row in ingestrowList)
                                        {
                                            //先比较各个ingest上所checkin的总时间   4+1 这种模式下  当1做的是长任务 所以需要比较时长 这样比较公平
                                            try
                                            {
                                                int tasktimeLenFrameNew = ConvertFrame(row.ColTaskTimeLen, 0);
                                                if (tasktimeLenFrame > tasktimeLenFrameNew) //如果新的ingest 做的任务时间 小于当前的任务时间  那么证明 新的ingest server 比较空闲
                                                {
                                                    tasktimeLenFrame = tasktimeLenFrameNew;
                                                    rowfind = row;
                                                }
                                            }
                                            catch (Exception ee)
                                            {
                                                WriteLogNew.writeLog("获取最佳avid ingest server 时长 异常！" + ee.ToString(), logpath, "error");
                                            }
                                            WriteLogNew.writeLog("获取最佳avid ingest server！" + rowfind.ColAvidinName + "IP:" + rowfind.ColIP + " 当前任务时长:" + rowfind.ColTaskTimeLen, logpath, "info");

                                            kk++;
                                        }

                                    } // if (taskcount==0) 

                                    //找到当前执行任务最少的 avidingest  将任务下发给它
                                    try
                                    {
                                        File.Copy(newavidingestxml, rowfind.ColScanPath + "\\" + Path.GetFileName(newavidingestxml), true);
                                        WriteLogNew.writeLog("下发任务到：" + rowfind.ColAvidinName + " IP:" + rowfind.ColIP, logpath, "info");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "下发任务到：" + rowfind.ColAvidinName + " IP:" + rowfind.ColIP + "\n");
                                        //将文件设置成已处理
                                        File.Copy(mediamd5, newlocalmd5file, true);
                                        WriteLogNew.writeLog("将文件设置成已处理状态!" + Path.GetFileName(mediamd5), logpath, "info");
                                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "将文件设置成已处理状态!" + Path.GetFileName(mediamd5) + "\n");
                                        //try
                                        //{
                                        //    File.Delete(mediamd5);
                                        //    WriteLogNew.writeLog("将MD5文件删除:" + Path.GetFileName(mediamd5), logpath, "info");
                                        //}
                                        //catch (Exception ee)
                                        //{
                                        //    WriteLogNew.writeLog("将MD5文件删除异常:" + Path.GetFileName(mediamd5)+ee.ToString(), logpath, "info");
                                        //    Thread.Sleep(1000);
                                        //    try
                                        //    {
                                        //        File.Delete(mediamd5);
                                        //        WriteLogNew.writeLog("将MD5文件删除:" + Path.GetFileName(mediamd5), logpath, "info");
                                        //    }
                                        //    catch (Exception eet)
                                        //    {
                                        //        WriteLogNew.writeLog("将MD5文件删除异常:" + Path.GetFileName(mediamd5) + eet.ToString(), logpath, "info");
                                        //    }

                                        //} //第一次异常

                                        filedelteTimes(mediamd5);
                                    }
                                    catch (Exception ee)
                                    {
                                        WriteLogNew.writeLog("下发任务异常:"+ee.ToString(),logpath,"error");
                                    }
                                    Thread.Sleep(4000);
                                }
                                else
                                {
                                    //加入短信报警中
                                    string warnlog = "avidin 调度素材找不到在线的avidingest server!";
                                    createSMStxt(warnlog);
                                    try
                                    {
                                        //File.Delete(newcountfile);
                                        //WriteLogNew.writeLog("删除count文件成功!" + newcountfile, logpath, "info");
                                        filedelteTimes(newcountfile);    
                     
                                        File.Move(newmxffilepath,mxffilepath );
                                        WriteLogNew.writeLog("将素材" + newmxffilepath + "修改名称:" + mxffilepath, logpath, "info");
                                        Thread.Sleep(10000);
                                    }
                                    catch (Exception ee)
                                    {
                                        WriteLogNew.writeLog("找不到在线的server 时处理异常:" +ee.ToString(), logpath, "error");
                                    }
                                }
                            }
                            catch (Exception ee)
                            {
                                WriteLogNew.writeLog("将原素材修改名称:" + mxffilepath+ee.ToString(), logpath, "error");
                            }

                        } //xnews 系统

                    }//foreach (string mediaxml in mediaxmllists)
                } //foreach (MediaInfos mi in mediainfoLists)
                Thread.Sleep(Properties.Settings.Default.scanVideoInterval);
            }
        }

        private void createSMStxt(string loginfo)
        {
            //生成txt短信报警
            //生成短信txt  //第一行手机号码 第二行内容
            string smspath = "";
            string smstextpath = Properties.Settings.Default.smspath;
            if (!Directory.Exists(smstextpath))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\smstxt");
                smspath = Application.StartupPath + "\\smstxt" + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            }
            else
            {
                smspath = smstextpath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".txt";
            }

            System.IO.StreamWriter swfn = System.IO.File.AppendText(smspath);
            string writelinetxt = "";
        

            foreach (string phone in Properties.Settings.Default.mobilePhones)
            {
                string[] mp = phone.Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries); //号码；姓名
                writelinetxt = mp[0] + ";" + writelinetxt;
            }

            swfn.WriteLine(writelinetxt);
            swfn.WriteLine(loginfo);
            swfn.Flush();
            swfn.Close();
        }
       
        /// <summary>
        /// xnews系统获取interplay路径
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        private string getInterplayWithXnews(string xmlpath)
        {
            string result = "";
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(xmlpath);
                XmlNode root = doc.DocumentElement;
                XmlNode SupplierNode = root.SelectSingleNode("//Supplier");
                try
                {
                    result = htrels[SupplierNode.InnerText].ToString();
                    WriteLogNew.writeLog("获取到了interplay的路径信息!" + result, logpath, "info");
                    SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "获取到了interplay的路径信息!" + result+"\n");
                }
                catch (Exception ee)
                {
                    WriteLogNew.writeLog("XNEWS" + "未能获取到interplay的路径信息!" + SupplierNode.InnerText + ee.ToString(), logpath, "error");
                    SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " + "未获取到了interplay的路径信息!" + result+"\n");
                }
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("XNEWS" + "获取interplay信息异常!" + ee.ToString(), logpath, "error");
            }
            return result;
        }
        /// <summary>
        /// 获取interplay的路径信息
        /// </summary>
        /// <param name="xmlpath"></param>
        /// <returns></returns>
        private string getInterplayPath(string xmlpath)
        {
            string result = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlpath);
                XmlNode root = doc.DocumentElement;

                XmlNode sourceSystemNode = root.SelectSingleNode("//sourceSystem");
                if (sourceSystemNode.InnerText.Equals("MediaManager"))
                {
                    //那么就按照 XInterplayPath 节点内容来查找对应的interplay路径
                    XmlNode XInterplayPathNode = root.SelectSingleNode("//XInterplayPath");
                    try
                    {
                        string[] keys = XInterplayPathNode.InnerText.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (keys.Length > 0)
                        {
                            result = htrels[keys[0]].ToString();
                        }
                        else
                        {
                            result = htrels[XInterplayPathNode.InnerText].ToString();
                        }
                        WriteLogNew.writeLog("获取到了interplay的路径信息!"+result, logpath, "info");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"获取到了interplay的路径信息!"+result+"\n");
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog(sourceSystemNode.InnerText + "未能获取到interplay的路径信息!" + XInterplayPathNode.InnerText + ee.ToString(), logpath, "error");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"未获取到了interplay的路径信息!" + result+"\n");
                    }
                }
                else if (sourceSystemNode.InnerText.Equals("XnewsSearch"))//XnewsSearch 
                {
                    XmlNode sitenode = root.SelectSingleNode("//site");

                    try
                    {
                        result = htrels[sitenode.InnerText].ToString();
                        WriteLogNew.writeLog("获取到了interplay的路径信息!" + result, logpath, "info");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"获取到了interplay的路径信息!" + result+"\n");
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog(sourceSystemNode.InnerText + "未能获取到interplay的路径信息!" + sitenode.InnerText + ee.ToString(), logpath, "error");
                        SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+sourceSystemNode.InnerText + "未能获取到interplay的路径信息!" + sitenode.InnerText+"\n");
                    }
                }
            }
            catch (Exception ee)
            {
                WriteLogNew.writeLog("获取interplay Path异常:" + ee.ToString(), logpath, "error");
                SetText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +" "+"获取interplay Path异常!\n");
                 
            }
            return result;
        }

        private void timer_check_Tick(object sender, EventArgs e)
        {
            foreach (DataSetMain.avidinRow row in this.dataSetMain.avidin.Rows)
            {
                //获取当前各个avidingest的任务数量
                try
                {
                    string[] files = Directory.GetFiles(row.ColScanPath, "*.xml", SearchOption.TopDirectoryOnly);
                    //获取总任务的时长
                   
                    int timeframe =0;

                    foreach(string filet in files)
                    {
                        string localTimeLenFile = Application.StartupPath + "\\timeLenFile"+"\\"+Path.GetFileName(filet);
                        if (File.Exists(localTimeLenFile))
                        {
                           //
                        }
                        else
                        {
                            File.Copy(filet, localTimeLenFile,true);
                            WriteLogNew.writeLog("复制素材时长文件到本地成功!"+filet,logpath,"info");
                        }

                        XmlDocument doc = new XmlDocument();
                        doc.Load(localTimeLenFile);
                        XmlNode root = doc.DocumentElement;
                        XmlNode durationNode = root.SelectSingleNode("//duration");
                        try
                        {
                            timeframe += ConvertFrame(durationNode.InnerText, 0);
                        }
                        catch (Exception ee)
                        {
                            WriteLogNew.writeLog("转化素材时间到帧异常!" + durationNode.InnerText + ee.ToString(), logpath, "error");
                        }
                    }
                    
                    try
                    {
                        row.ColTaskTimeLen = ConvertTime(timeframe);
                    }
                    catch (Exception ee)
                    {
                        WriteLogNew.writeLog("转化素材帧到tc时间异常!" + timeframe.ToString() + ee.ToString(), logpath, "error");
                    }

                    row.ColTaskCount = files.Length;
                    row.ColCheckTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    ////判断avid ingest 是否离线 
                    string livename = row.ColScanPath + "\\" + row.ColAvidinName + ".live";
                    if (!Directory.Exists(Application.StartupPath + "\\livefiles"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "\\livefiles");
                    }
                    string locallivename = Application.StartupPath + "\\livefiles" + "\\" + row.ColAvidinName + ".xml";
                    if (File.Exists(livename))
                    {
                        //判断文件是否正在被使用
                        if (!IsFileInUse(livename))
                        {
                            File.Copy(livename, locallivename, true);
                            XmlDocument doclive = new XmlDocument();
                            doclive.Load(locallivename);
                            XmlNode timeNode = doclive.SelectSingleNode("//time");
                            //超过1分钟没有响应就认为该avidingest 死机了 就不会给他发认为

                            DateTime dtxml = DateTime.ParseExact(timeNode.InnerText, "yyyy-MM-dd HH:mm:ss.fff", System.Globalization.CultureInfo.CurrentCulture);
                            if (DateTime.Now > dtxml.AddSeconds(60))  //60s 还没有收到消息 认为该台server 已经离线
                            {
                                row.ColStatus = "离线";
                            }
                            else
                            {
                                row.ColStatus = "在线";
                            }
                        }
                       
                    }
                 

                }
                catch (Exception ee)
                {
                    WriteLogNew.writeLog("获取当前各个avidingest的任务数量 任务状态 异常!" + ee.ToString(), logpath, "error");
                }

            }
        }



        private void filedelteTimes(string filepath)
        {
            for (int i = 0; i < 10; i++)
            {
                if (!File.Exists(filepath))
                {
                    break;
                }
                try
                {
                    File.Delete(filepath);
                    WriteLogNew.writeLog("删除文件成功!" + filepath, logpath, "info");
                    break;
                }
                catch (Exception ee)
                {

                    WriteLogNew.writeLog("删除文件异常!" + filepath + ee.ToString(), logpath, "error");
                    Thread.Sleep(1000);
                }
            }

        }
       

        public bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用  
        }

    }
}
