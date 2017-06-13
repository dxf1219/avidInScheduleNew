namespace avidInSchedule
{
    partial class Form_main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_main));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.avidinBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetMain = new avidInSchedule.DataSetMain();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.timer_check = new System.Windows.Forms.Timer(this.components);
            this.colAvidinNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIPDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScanPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTaskCountDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCheckTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColIfTakeLongTask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColTaskTimeLen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.avidinBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetMain)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(824, 471);
            this.splitContainer1.SplitterDistance = 117;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAvidinNameDataGridViewTextBoxColumn,
            this.colIPDataGridViewTextBoxColumn,
            this.colScanPathDataGridViewTextBoxColumn,
            this.colTaskCountDataGridViewTextBoxColumn,
            this.colStatusDataGridViewTextBoxColumn,
            this.ColCheckTime,
            this.ColIfTakeLongTask,
            this.ColTaskTimeLen});
            this.dataGridView1.DataSource = this.avidinBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(824, 117);
            this.dataGridView1.TabIndex = 0;
            // 
            // avidinBindingSource
            // 
            this.avidinBindingSource.DataMember = "avidin";
            this.avidinBindingSource.DataSource = this.dataSetMain;
            // 
            // dataSetMain
            // 
            this.dataSetMain.DataSetName = "DataSetMain";
            this.dataSetMain.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(824, 350);
            this.richTextBox1.TabIndex = 56;
            this.richTextBox1.Text = "";
            // 
            // timer_check
            // 
            this.timer_check.Interval = 1000;
            this.timer_check.Tick += new System.EventHandler(this.timer_check_Tick);
            // 
            // colAvidinNameDataGridViewTextBoxColumn
            // 
            this.colAvidinNameDataGridViewTextBoxColumn.DataPropertyName = "ColAvidinName";
            this.colAvidinNameDataGridViewTextBoxColumn.FillWeight = 120F;
            this.colAvidinNameDataGridViewTextBoxColumn.HeaderText = "AvidIn名称";
            this.colAvidinNameDataGridViewTextBoxColumn.Name = "colAvidinNameDataGridViewTextBoxColumn";
            this.colAvidinNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // colIPDataGridViewTextBoxColumn
            // 
            this.colIPDataGridViewTextBoxColumn.DataPropertyName = "ColIP";
            this.colIPDataGridViewTextBoxColumn.HeaderText = "IP";
            this.colIPDataGridViewTextBoxColumn.Name = "colIPDataGridViewTextBoxColumn";
            this.colIPDataGridViewTextBoxColumn.ReadOnly = true;
            this.colIPDataGridViewTextBoxColumn.Width = 90;
            // 
            // colScanPathDataGridViewTextBoxColumn
            // 
            this.colScanPathDataGridViewTextBoxColumn.DataPropertyName = "ColScanPath";
            this.colScanPathDataGridViewTextBoxColumn.FillWeight = 150F;
            this.colScanPathDataGridViewTextBoxColumn.HeaderText = "扫描路径";
            this.colScanPathDataGridViewTextBoxColumn.Name = "colScanPathDataGridViewTextBoxColumn";
            this.colScanPathDataGridViewTextBoxColumn.ReadOnly = true;
            this.colScanPathDataGridViewTextBoxColumn.Width = 150;
            // 
            // colTaskCountDataGridViewTextBoxColumn
            // 
            this.colTaskCountDataGridViewTextBoxColumn.DataPropertyName = "ColTaskCount";
            this.colTaskCountDataGridViewTextBoxColumn.HeaderText = "数量";
            this.colTaskCountDataGridViewTextBoxColumn.Name = "colTaskCountDataGridViewTextBoxColumn";
            this.colTaskCountDataGridViewTextBoxColumn.ReadOnly = true;
            this.colTaskCountDataGridViewTextBoxColumn.Width = 70;
            // 
            // colStatusDataGridViewTextBoxColumn
            // 
            this.colStatusDataGridViewTextBoxColumn.DataPropertyName = "ColStatus";
            this.colStatusDataGridViewTextBoxColumn.HeaderText = "状态";
            this.colStatusDataGridViewTextBoxColumn.Name = "colStatusDataGridViewTextBoxColumn";
            this.colStatusDataGridViewTextBoxColumn.ReadOnly = true;
            this.colStatusDataGridViewTextBoxColumn.Width = 60;
            // 
            // ColCheckTime
            // 
            this.ColCheckTime.DataPropertyName = "ColCheckTime";
            this.ColCheckTime.FillWeight = 150F;
            this.ColCheckTime.HeaderText = "检测时间";
            this.ColCheckTime.Name = "ColCheckTime";
            this.ColCheckTime.ReadOnly = true;
            this.ColCheckTime.Width = 150;
            // 
            // ColIfTakeLongTask
            // 
            this.ColIfTakeLongTask.DataPropertyName = "ColIfTakeLongTask";
            this.ColIfTakeLongTask.HeaderText = "长任务";
            this.ColIfTakeLongTask.Name = "ColIfTakeLongTask";
            this.ColIfTakeLongTask.ReadOnly = true;
            this.ColIfTakeLongTask.Width = 70;
            // 
            // ColTaskTimeLen
            // 
            this.ColTaskTimeLen.DataPropertyName = "ColTaskTimeLen";
            this.ColTaskTimeLen.HeaderText = "总时长";
            this.ColTaskTimeLen.Name = "ColTaskTimeLen";
            this.ColTaskTimeLen.ReadOnly = true;
            this.ColTaskTimeLen.Width = 90;
            // 
            // Form_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 471);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_main";
            this.Text = "AvidIn调度";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.avidinBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource avidinBindingSource;
        private DataSetMain dataSetMain;
        private System.Windows.Forms.Timer timer_check;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvidinNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIPDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScanPathDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTaskCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCheckTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColIfTakeLongTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColTaskTimeLen;

    }
}

