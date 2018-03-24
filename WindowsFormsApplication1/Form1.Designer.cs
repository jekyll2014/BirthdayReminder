namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox_bdList = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox_bdList
            // 
            this.textBox_bdList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_bdList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox_bdList.Location = new System.Drawing.Point(0, 0);
            this.textBox_bdList.Multiline = true;
            this.textBox_bdList.Name = "textBox_bdList";
            this.textBox_bdList.ReadOnly = true;
            this.textBox_bdList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_bdList.Size = new System.Drawing.Size(584, 262);
            this.textBox_bdList.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 262);
            this.Controls.Add(this.textBox_bdList);
            this.Name = "Form1";
            this.Text = "BirthdayReminder";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_bdList;
    }
}

