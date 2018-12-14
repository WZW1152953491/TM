namespace TMCurve
{
    partial class shuxing
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.sxname = new System.Windows.Forms.GroupBox();
            this.sxYtitle = new System.Windows.Forms.TextBox();
            this.sxXtitle = new System.Windows.Forms.TextBox();
            this.sxtitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.notLine = new System.Windows.Forms.RadioButton();
            this.Line = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Wellzero = new System.Windows.Forms.TextBox();
            this.sxname.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, 168);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "保存并退出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(82, 168);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "恢复默认";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(163, 168);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "退出";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // sxname
            // 
            this.sxname.Controls.Add(this.sxYtitle);
            this.sxname.Controls.Add(this.sxXtitle);
            this.sxname.Controls.Add(this.sxtitle);
            this.sxname.Controls.Add(this.label3);
            this.sxname.Controls.Add(this.label2);
            this.sxname.Controls.Add(this.label1);
            this.sxname.Location = new System.Drawing.Point(0, 0);
            this.sxname.Name = "sxname";
            this.sxname.Size = new System.Drawing.Size(373, 85);
            this.sxname.TabIndex = 3;
            this.sxname.TabStop = false;
            this.sxname.Text = "标题";
            // 
            // sxYtitle
            // 
            this.sxYtitle.Location = new System.Drawing.Point(244, 54);
            this.sxYtitle.Name = "sxYtitle";
            this.sxYtitle.Size = new System.Drawing.Size(106, 21);
            this.sxYtitle.TabIndex = 5;
            // 
            // sxXtitle
            // 
            this.sxXtitle.Location = new System.Drawing.Point(62, 54);
            this.sxXtitle.Name = "sxXtitle";
            this.sxXtitle.Size = new System.Drawing.Size(106, 21);
            this.sxXtitle.TabIndex = 4;
            // 
            // sxtitle
            // 
            this.sxtitle.Location = new System.Drawing.Point(62, 20);
            this.sxtitle.Name = "sxtitle";
            this.sxtitle.Size = new System.Drawing.Size(288, 21);
            this.sxtitle.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(191, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Y轴标题";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "X轴标题";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "曲线标题";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.notLine);
            this.groupBox1.Controls.Add(this.Line);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Wellzero);
            this.groupBox1.Location = new System.Drawing.Point(5, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 71);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "井口位置";
            // 
            // notLine
            // 
            this.notLine.AutoSize = true;
            this.notLine.Location = new System.Drawing.Point(153, 44);
            this.notLine.Name = "notLine";
            this.notLine.Size = new System.Drawing.Size(35, 16);
            this.notLine.TabIndex = 3;
            this.notLine.TabStop = true;
            this.notLine.Text = "点";
            this.notLine.UseVisualStyleBackColor = true;
            this.notLine.CheckedChanged += new System.EventHandler(this.notLine_CheckedChanged);
            // 
            // Line
            // 
            this.Line.AutoSize = true;
            this.Line.Location = new System.Drawing.Point(88, 44);
            this.Line.Name = "Line";
            this.Line.Size = new System.Drawing.Size(35, 16);
            this.Line.TabIndex = 2;
            this.Line.TabStop = true;
            this.Line.Text = "线";
            this.Line.UseVisualStyleBackColor = true;
            this.Line.CheckedChanged += new System.EventHandler(this.Line_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "曲线样式：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "井口位置(m)";
            // 
            // Wellzero
            // 
            this.Wellzero.Location = new System.Drawing.Point(88, 17);
            this.Wellzero.Name = "Wellzero";
            this.Wellzero.Size = new System.Drawing.Size(100, 21);
            this.Wellzero.TabIndex = 0;
            this.Wellzero.Text = "150";
            this.Wellzero.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Wellzero_KeyPress);
            // 
            // shuxing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 205);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.sxname);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "shuxing";
            this.Text = "图形属性设置";
            this.Load += new System.EventHandler(this.shuxing_Load);
            this.sxname.ResumeLayout(false);
            this.sxname.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox sxname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox sxXtitle;
        public System.Windows.Forms.TextBox sxtitle;
        public System.Windows.Forms.TextBox sxYtitle;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox Wellzero;
        public System.Windows.Forms.RadioButton notLine;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.RadioButton Line;
    }
}