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
            this.button_Start = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_SaveRfaResource = new System.Windows.Forms.Button();
            this.textBox_Log = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "建模任务监听状态：";
            // 
            // button_Start
            // 
            this.button_Start.Location = new System.Drawing.Point(14, 45);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(82, 34);
            this.button_Start.TabIndex = 1;
            this.button_Start.Text = "开始";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Enabled = false;
            this.button_Stop.Location = new System.Drawing.Point(102, 45);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(82, 34);
            this.button_Stop.TabIndex = 2;
            this.button_Stop.Text = "停止";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_SaveRfaResource
            // 
            this.button_SaveRfaResource.Location = new System.Drawing.Point(190, 45);
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
            // Form_SiloModelingTaskClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 613);
            this.Controls.Add(this.textBox_Log);
            this.Controls.Add(this.button_SaveRfaResource);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_Start);
            this.Controls.Add(this.label1);
            this.Name = "Form_SiloModelingTaskClient";
            this.Text = "Form_SiloModelingTaskClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_SiloModelingTaskClient_FormClosing);
            this.Load += new System.EventHandler(this.Form_SiloModelingTaskClient_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_SaveRfaResource;
        private System.Windows.Forms.TextBox textBox_Log;
    }
}
