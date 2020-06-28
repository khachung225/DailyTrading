namespace DailyTrading
{
    partial class PError
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
            this.components = new System.ComponentModel.Container();
            this.DoThi_Error = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // DoThi_Error
            // 
            this.DoThi_Error.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DoThi_Error.Location = new System.Drawing.Point(16, 15);
            this.DoThi_Error.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.DoThi_Error.Name = "DoThi_Error";
            this.DoThi_Error.ScrollGrace = 0D;
            this.DoThi_Error.ScrollMaxX = 0D;
            this.DoThi_Error.ScrollMaxY = 0D;
            this.DoThi_Error.ScrollMaxY2 = 0D;
            this.DoThi_Error.ScrollMinX = 0D;
            this.DoThi_Error.ScrollMinY = 0D;
            this.DoThi_Error.ScrollMinY2 = 0D;
            this.DoThi_Error.Size = new System.Drawing.Size(896, 348);
            this.DoThi_Error.TabIndex = 5;
            // 
            // Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(949, 387);
            this.Controls.Add(this.DoThi_Error);
            this.Name = "Error";
            this.Text = "Error";
            this.Load += new System.EventHandler(this.Error_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ZedGraph.ZedGraphControl DoThi_Error;
    }
}