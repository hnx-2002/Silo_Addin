namespace SiloModelingTaskClient
{
    partial class Form_SiloModelingTaskClient
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.button_GetNewTasks = new System.Windows.Forms.Button();
            this.button_ExecuteModeling = new System.Windows.Forms.Button();
            this.button_SaveRfaResource = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.label_SiloName = new System.Windows.Forms.Label();
            this.comboBox_DictSilo = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "建模任务操作：";
            // 
            // button_GetNewTasks
            // 
            this.button_GetNewTasks.Location = new System.Drawing.Point(14, 45);
            this.button_GetNewTasks.Name = "button_GetNewTasks";
            this.button_GetNewTasks.Size = new System.Drawing.Size(92, 34);
            this.button_GetNewTasks.TabIndex = 1;
            this.button_GetNewTasks.Text = "获取新任务";
            this.button_GetNewTasks.UseVisualStyleBackColor = true;
            this.button_GetNewTasks.Click += new System.EventHandler(this.button_GetNewTasks_Click);
            // 
            // button_ExecuteModeling
            // 
            this.button_ExecuteModeling.Location = new System.Drawing.Point(112, 45);
            this.button_ExecuteModeling.Name = "button_ExecuteModeling";
            this.button_ExecuteModeling.Size = new System.Drawing.Size(92, 34);
            this.button_ExecuteModeling.TabIndex = 2;
            this.button_ExecuteModeling.Text = "执行建模";
            this.button_ExecuteModeling.UseVisualStyleBackColor = true;
            this.button_ExecuteModeling.Click += new System.EventHandler(this.button_ExecuteModeling_Click);
            // 
            // button_SaveRfaResource
            // 
            this.button_SaveRfaResource.Location = new System.Drawing.Point(454, 45);
            this.button_SaveRfaResource.Name = "button_SaveRfaResource";
            this.button_SaveRfaResource.Size = new System.Drawing.Size(110, 34);
            this.button_SaveRfaResource.TabIndex = 4;
            this.button_SaveRfaResource.Text = "保存族资源";
            this.button_SaveRfaResource.UseVisualStyleBackColor = true;
            this.button_SaveRfaResource.Click += new System.EventHandler(this.button_SaveRfaResource_Click);
            // 
            // textBox_Log
            // 
            this.textBox_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_Log.Location = new System.Drawing.Point(12, 93);
            this.textBox_Log.Multiline = true;
            this.textBox_Log.Name = "textBox_Log";
            this.textBox_Log.ReadOnly = true;
            this.textBox_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Log.Size = new System.Drawing.Size(668, 508);
            this.textBox_Log.TabIndex = 3;
            // 
            // label_SiloName
            // 
            this.label_SiloName.AutoSize = true;
            this.label_SiloName.Location = new System.Drawing.Point(210, 56);
            this.label_SiloName.Name = "label_SiloName";
            this.label_SiloName.Size = new System.Drawing.Size(53, 12);
            this.label_SiloName.TabIndex = 5;
            this.label_SiloName.Text = "库型：";
            // 
            // comboBox_DictSilo
            // 
            this.comboBox_DictSilo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_DictSilo.FormattingEnabled = true;
            this.comboBox_DictSilo.Location = new System.Drawing.Point(269, 51);
            this.comboBox_DictSilo.Name = "comboBox_DictSilo";
            this.comboBox_DictSilo.Size = new System.Drawing.Size(179, 20);
            this.comboBox_DictSilo.TabIndex = 6;
            this.comboBox_DictSilo.SelectedIndexChanged += new System.EventHandler(this.comboBox_DictSilo_SelectedIndexChanged);
            // 
            // Form_SiloModelingTaskClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 613);
            this.Controls.Add(this.comboBox_DictSilo);
            this.Controls.Add(this.label_SiloName);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.button_SaveRfaResource);
            this.Controls.Add(this.button_ExecuteModeling);
            this.Controls.Add(this.button_GetNewTasks);
            this.Controls.Add(this.label1);
            this.Name = "Form_SiloModelingTaskClient";
            this.Text = "Form_SiloModelingTaskClient";
            this.Load += new System.EventHandler(this.Form_SiloModelingTaskClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_GetNewTasks;
        private System.Windows.Forms.Button button_ExecuteModeling;
        private System.Windows.Forms.Button button_SaveRfaResource;
        private System.Windows.Forms.TextBox textBox_Log;
        private System.Windows.Forms.Label label_SiloName;
        private System.Windows.Forms.ComboBox comboBox_DictSilo;
    }
}
