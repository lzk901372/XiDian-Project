namespace xd2
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cameraPic = new System.Windows.Forms.PictureBox();
            this.openCameraButton = new System.Windows.Forms.Button();
            this.shotButton = new System.Windows.Forms.Button();
            this.cameraZone = new System.Windows.Forms.GroupBox();
            this.radioButtonList = new System.Windows.Forms.Panel();
            this.faceStatus = new System.Windows.Forms.Label();
            this.testIDStatus = new System.Windows.Forms.Label();
            this.idStatus = new System.Windows.Forms.Label();
            this.idSelect = new System.Windows.Forms.RadioButton();
            this.faceSelect = new System.Windows.Forms.RadioButton();
            this.testIDSelect = new System.Windows.Forms.RadioButton();
            this.infoAvatar = new System.Windows.Forms.PictureBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.idLabel = new System.Windows.Forms.Label();
            this.idTextBox = new System.Windows.Forms.TextBox();
            this.testLocLabel = new System.Windows.Forms.Label();
            this.testLocTextBox = new System.Windows.Forms.TextBox();
            this.testAvatar = new System.Windows.Forms.PictureBox();
            this.testIDLabel = new System.Windows.Forms.Label();
            this.testIDTextBox = new System.Windows.Forms.TextBox();
            this.timeLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stuNumTextBox = new System.Windows.Forms.TextBox();
            this.stuNumLabel = new System.Windows.Forms.Label();
            this.testNumberLabel = new System.Windows.Forms.Label();
            this.testNumberTextbox = new System.Windows.Forms.TextBox();
            this.roomIDLabel = new System.Windows.Forms.Label();
            this.roomIDTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.faceNumberTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.levelTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.cameraPic)).BeginInit();
            this.cameraZone.SuspendLayout();
            this.radioButtonList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoAvatar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.testAvatar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cameraPic
            // 
            this.cameraPic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.cameraPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cameraPic.Location = new System.Drawing.Point(17, 38);
            this.cameraPic.Name = "cameraPic";
            this.cameraPic.Size = new System.Drawing.Size(431, 319);
            this.cameraPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.cameraPic.TabIndex = 0;
            this.cameraPic.TabStop = false;
            // 
            // openCameraButton
            // 
            this.openCameraButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.openCameraButton.Location = new System.Drawing.Point(17, 493);
            this.openCameraButton.Name = "openCameraButton";
            this.openCameraButton.Size = new System.Drawing.Size(256, 40);
            this.openCameraButton.TabIndex = 1;
            this.openCameraButton.Text = "打开摄像头";
            this.openCameraButton.UseVisualStyleBackColor = true;
            this.openCameraButton.Click += new System.EventHandler(this.openCameraButton_Click);
            // 
            // shotButton
            // 
            this.shotButton.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.shotButton.Location = new System.Drawing.Point(288, 376);
            this.shotButton.Name = "shotButton";
            this.shotButton.Size = new System.Drawing.Size(160, 157);
            this.shotButton.TabIndex = 2;
            this.shotButton.Text = "拍摄";
            this.shotButton.UseVisualStyleBackColor = true;
            this.shotButton.Click += new System.EventHandler(this.shotButton_Click);
            // 
            // cameraZone
            // 
            this.cameraZone.Controls.Add(this.radioButtonList);
            this.cameraZone.Controls.Add(this.cameraPic);
            this.cameraZone.Controls.Add(this.shotButton);
            this.cameraZone.Controls.Add(this.openCameraButton);
            this.cameraZone.Font = new System.Drawing.Font("Segoe UI Emoji", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cameraZone.Location = new System.Drawing.Point(663, 24);
            this.cameraZone.Name = "cameraZone";
            this.cameraZone.Size = new System.Drawing.Size(463, 545);
            this.cameraZone.TabIndex = 3;
            this.cameraZone.TabStop = false;
            this.cameraZone.Text = "摄像区域";
            // 
            // radioButtonList
            // 
            this.radioButtonList.Controls.Add(this.faceStatus);
            this.radioButtonList.Controls.Add(this.testIDStatus);
            this.radioButtonList.Controls.Add(this.idStatus);
            this.radioButtonList.Controls.Add(this.idSelect);
            this.radioButtonList.Controls.Add(this.faceSelect);
            this.radioButtonList.Controls.Add(this.testIDSelect);
            this.radioButtonList.Font = new System.Drawing.Font("Segoe UI Emoji", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonList.Location = new System.Drawing.Point(17, 376);
            this.radioButtonList.Name = "radioButtonList";
            this.radioButtonList.Size = new System.Drawing.Size(256, 101);
            this.radioButtonList.TabIndex = 8;
            // 
            // faceStatus
            // 
            this.faceStatus.AutoSize = true;
            this.faceStatus.Location = new System.Drawing.Point(158, 73);
            this.faceStatus.Name = "faceStatus";
            this.faceStatus.Size = new System.Drawing.Size(64, 24);
            this.faceStatus.TabIndex = 10;
            this.faceStatus.Text = "待完成";
            // 
            // testIDStatus
            // 
            this.testIDStatus.AutoSize = true;
            this.testIDStatus.Location = new System.Drawing.Point(158, 39);
            this.testIDStatus.Name = "testIDStatus";
            this.testIDStatus.Size = new System.Drawing.Size(64, 24);
            this.testIDStatus.TabIndex = 9;
            this.testIDStatus.Text = "待完成";
            // 
            // idStatus
            // 
            this.idStatus.AutoSize = true;
            this.idStatus.Location = new System.Drawing.Point(158, 5);
            this.idStatus.Name = "idStatus";
            this.idStatus.Size = new System.Drawing.Size(64, 24);
            this.idStatus.TabIndex = 8;
            this.idStatus.Text = "待完成";
            // 
            // idSelect
            // 
            this.idSelect.AutoSize = true;
            this.idSelect.Checked = true;
            this.idSelect.Location = new System.Drawing.Point(13, 3);
            this.idSelect.Name = "idSelect";
            this.idSelect.Size = new System.Drawing.Size(140, 28);
            this.idSelect.TabIndex = 5;
            this.idSelect.TabStop = true;
            this.idSelect.Text = "1 身份证拍摄";
            this.idSelect.UseVisualStyleBackColor = true;
            // 
            // faceSelect
            // 
            this.faceSelect.AutoSize = true;
            this.faceSelect.Location = new System.Drawing.Point(13, 71);
            this.faceSelect.Name = "faceSelect";
            this.faceSelect.Size = new System.Drawing.Size(122, 28);
            this.faceSelect.TabIndex = 7;
            this.faceSelect.Text = "3 人脸拍摄";
            this.faceSelect.UseVisualStyleBackColor = true;
            // 
            // testIDSelect
            // 
            this.testIDSelect.AutoSize = true;
            this.testIDSelect.Location = new System.Drawing.Point(13, 37);
            this.testIDSelect.Name = "testIDSelect";
            this.testIDSelect.Size = new System.Drawing.Size(140, 28);
            this.testIDSelect.TabIndex = 6;
            this.testIDSelect.Text = "2 准考证拍摄";
            this.testIDSelect.UseVisualStyleBackColor = true;
            // 
            // infoAvatar
            // 
            this.infoAvatar.Location = new System.Drawing.Point(21, 50);
            this.infoAvatar.Name = "infoAvatar";
            this.infoAvatar.Size = new System.Drawing.Size(110, 143);
            this.infoAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.infoAvatar.TabIndex = 4;
            this.infoAvatar.TabStop = false;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nameLabel.Location = new System.Drawing.Point(151, 37);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(56, 28);
            this.nameLabel.TabIndex = 5;
            this.nameLabel.Text = "姓名";
            // 
            // nameTextBox
            // 
            this.nameTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.nameTextBox.Location = new System.Drawing.Point(153, 68);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ReadOnly = true;
            this.nameTextBox.Size = new System.Drawing.Size(436, 36);
            this.nameTextBox.TabIndex = 6;
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.idLabel.Location = new System.Drawing.Point(145, 121);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(100, 28);
            this.idLabel.TabIndex = 7;
            this.idLabel.Text = "身份证号";
            // 
            // idTextBox
            // 
            this.idTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.idTextBox.Location = new System.Drawing.Point(153, 157);
            this.idTextBox.Name = "idTextBox";
            this.idTextBox.ReadOnly = true;
            this.idTextBox.Size = new System.Drawing.Size(436, 36);
            this.idTextBox.TabIndex = 8;
            // 
            // testLocLabel
            // 
            this.testLocLabel.AutoSize = true;
            this.testLocLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testLocLabel.Location = new System.Drawing.Point(149, 37);
            this.testLocLabel.Name = "testLocLabel";
            this.testLocLabel.Size = new System.Drawing.Size(56, 28);
            this.testLocLabel.TabIndex = 9;
            this.testLocLabel.Text = "考点";
            // 
            // testLocTextBox
            // 
            this.testLocTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.testLocTextBox.Location = new System.Drawing.Point(153, 72);
            this.testLocTextBox.Name = "testLocTextBox";
            this.testLocTextBox.ReadOnly = true;
            this.testLocTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.testLocTextBox.Size = new System.Drawing.Size(110, 36);
            this.testLocTextBox.TabIndex = 10;
            // 
            // testAvatar
            // 
            this.testAvatar.Location = new System.Drawing.Point(19, 49);
            this.testAvatar.Name = "testAvatar";
            this.testAvatar.Size = new System.Drawing.Size(110, 146);
            this.testAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.testAvatar.TabIndex = 12;
            this.testAvatar.TabStop = false;
            // 
            // testIDLabel
            // 
            this.testIDLabel.AutoSize = true;
            this.testIDLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testIDLabel.Location = new System.Drawing.Point(145, 116);
            this.testIDLabel.Name = "testIDLabel";
            this.testIDLabel.Size = new System.Drawing.Size(100, 28);
            this.testIDLabel.TabIndex = 13;
            this.testIDLabel.Text = "准考证号";
            // 
            // testIDTextBox
            // 
            this.testIDTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.testIDTextBox.Location = new System.Drawing.Point(153, 154);
            this.testIDTextBox.Name = "testIDTextBox";
            this.testIDTextBox.ReadOnly = true;
            this.testIDTextBox.Size = new System.Drawing.Size(250, 36);
            this.testIDTextBox.TabIndex = 14;
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.timeLabel.Location = new System.Drawing.Point(229, 539);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(62, 18);
            this.timeLabel.TabIndex = 4;
            this.timeLabel.Text = "label2";
            this.timeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.infoAvatar);
            this.groupBox1.Controls.Add(this.nameLabel);
            this.groupBox1.Controls.Add(this.nameTextBox);
            this.groupBox1.Controls.Add(this.idLabel);
            this.groupBox1.Controls.Add(this.idTextBox);
            this.groupBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(28, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(613, 225);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "身份证";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.levelTextBox);
            this.groupBox2.Controls.Add(this.levelLabel);
            this.groupBox2.Controls.Add(this.stuNumTextBox);
            this.groupBox2.Controls.Add(this.stuNumLabel);
            this.groupBox2.Controls.Add(this.testNumberLabel);
            this.groupBox2.Controls.Add(this.testNumberTextbox);
            this.groupBox2.Controls.Add(this.roomIDLabel);
            this.groupBox2.Controls.Add(this.roomIDTextBox);
            this.groupBox2.Controls.Add(this.testAvatar);
            this.groupBox2.Controls.Add(this.testLocLabel);
            this.groupBox2.Controls.Add(this.testIDTextBox);
            this.groupBox2.Controls.Add(this.testLocTextBox);
            this.groupBox2.Controls.Add(this.testIDLabel);
            this.groupBox2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(28, 255);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(613, 218);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "准考证";
            // 
            // stuNumTextBox
            // 
            this.stuNumTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.stuNumTextBox.Location = new System.Drawing.Point(419, 154);
            this.stuNumTextBox.Name = "stuNumTextBox";
            this.stuNumTextBox.ReadOnly = true;
            this.stuNumTextBox.Size = new System.Drawing.Size(170, 36);
            this.stuNumTextBox.TabIndex = 22;
            // 
            // stuNumLabel
            // 
            this.stuNumLabel.AutoSize = true;
            this.stuNumLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stuNumLabel.Location = new System.Drawing.Point(414, 116);
            this.stuNumLabel.Name = "stuNumLabel";
            this.stuNumLabel.Size = new System.Drawing.Size(56, 28);
            this.stuNumLabel.TabIndex = 21;
            this.stuNumLabel.Text = "学号";
            // 
            // testNumberLabel
            // 
            this.testNumberLabel.AutoSize = true;
            this.testNumberLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.testNumberLabel.Location = new System.Drawing.Point(380, 37);
            this.testNumberLabel.Name = "testNumberLabel";
            this.testNumberLabel.Size = new System.Drawing.Size(56, 28);
            this.testNumberLabel.TabIndex = 17;
            this.testNumberLabel.Text = "考号";
            // 
            // testNumberTextbox
            // 
            this.testNumberTextbox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.testNumberTextbox.Location = new System.Drawing.Point(385, 72);
            this.testNumberTextbox.Name = "testNumberTextbox";
            this.testNumberTextbox.ReadOnly = true;
            this.testNumberTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.testNumberTextbox.Size = new System.Drawing.Size(73, 36);
            this.testNumberTextbox.TabIndex = 18;
            // 
            // roomIDLabel
            // 
            this.roomIDLabel.AutoSize = true;
            this.roomIDLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roomIDLabel.Location = new System.Drawing.Point(274, 37);
            this.roomIDLabel.Name = "roomIDLabel";
            this.roomIDLabel.Size = new System.Drawing.Size(78, 28);
            this.roomIDLabel.TabIndex = 15;
            this.roomIDLabel.Text = "考场号";
            // 
            // roomIDTextBox
            // 
            this.roomIDTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roomIDTextBox.Location = new System.Drawing.Point(279, 72);
            this.roomIDTextBox.Name = "roomIDTextBox";
            this.roomIDTextBox.ReadOnly = true;
            this.roomIDTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.roomIDTextBox.Size = new System.Drawing.Size(89, 36);
            this.roomIDTextBox.TabIndex = 16;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(28, 479);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 28);
            this.button1.TabIndex = 17;
            this.button1.Text = "打开摄像头";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // faceNumberTextBox
            // 
            this.faceNumberTextBox.Location = new System.Drawing.Point(186, 479);
            this.faceNumberTextBox.Name = "faceNumberTextBox";
            this.faceNumberTextBox.Size = new System.Drawing.Size(49, 28);
            this.faceNumberTextBox.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(267, 484);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 18);
            this.label1.TabIndex = 19;
            // 
            // levelLabel
            // 
            this.levelLabel.AutoSize = true;
            this.levelLabel.Font = new System.Drawing.Font("Segoe UI Emoji", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.levelLabel.Location = new System.Drawing.Point(471, 37);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(56, 28);
            this.levelLabel.TabIndex = 23;
            this.levelLabel.Text = "级别";
            // 
            // levelTextBox
            // 
            this.levelTextBox.Font = new System.Drawing.Font("华文细黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.levelTextBox.Location = new System.Drawing.Point(476, 72);
            this.levelTextBox.Name = "levelTextBox";
            this.levelTextBox.ReadOnly = true;
            this.levelTextBox.Size = new System.Drawing.Size(113, 36);
            this.levelTextBox.TabIndex = 24;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1166, 589);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.faceNumberTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.cameraZone);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.cameraPic)).EndInit();
            this.cameraZone.ResumeLayout(false);
            this.radioButtonList.ResumeLayout(false);
            this.radioButtonList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoAvatar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.testAvatar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox cameraPic;
        private System.Windows.Forms.Button openCameraButton;
        private System.Windows.Forms.Button shotButton;
        private System.Windows.Forms.GroupBox cameraZone;
        private System.Windows.Forms.PictureBox infoAvatar;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.TextBox idTextBox;
        private System.Windows.Forms.Label testLocLabel;
        private System.Windows.Forms.TextBox testLocTextBox;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.PictureBox testAvatar;
        private System.Windows.Forms.Label testIDLabel;
        private System.Windows.Forms.TextBox testIDTextBox;
        private System.Windows.Forms.Panel radioButtonList;
        private System.Windows.Forms.RadioButton idSelect;
        private System.Windows.Forms.RadioButton faceSelect;
        private System.Windows.Forms.RadioButton testIDSelect;
        private System.Windows.Forms.Label faceStatus;
        private System.Windows.Forms.Label testIDStatus;
        private System.Windows.Forms.Label idStatus;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label testNumberLabel;
        private System.Windows.Forms.TextBox testNumberTextbox;
        private System.Windows.Forms.Label roomIDLabel;
        private System.Windows.Forms.TextBox roomIDTextBox;
        private System.Windows.Forms.TextBox stuNumTextBox;
        private System.Windows.Forms.Label stuNumLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox faceNumberTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox levelTextBox;
        private System.Windows.Forms.Label levelLabel;
    }
}

