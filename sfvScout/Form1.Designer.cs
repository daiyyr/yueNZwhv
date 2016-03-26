namespace widkeyPaperDiaper
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.loginB = new System.Windows.Forms.Button();
            this.autoB = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.logT = new System.Windows.Forms.RichTextBox();
            this.addB = new System.Windows.Forms.Button();
            this.deleteMail = new System.Windows.Forms.Button();
            this.inputT = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.testLog = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.deleteApp = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.appointmentGrid = new System.Windows.Forms.DataGridView();
            this.cardNoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cardPasswordDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chineseNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.japaneseNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.phoneDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.appointmentBindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.mailGrid = new System.Windows.Forms.DataGridView();
            this.emailDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.passwordDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.emailForshowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.appointmentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.appointmentBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.appointmentGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emailForshowBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // loginB
            // 
            this.loginB.Location = new System.Drawing.Point(331, 130);
            this.loginB.Margin = new System.Windows.Forms.Padding(4);
            this.loginB.Name = "loginB";
            this.loginB.Size = new System.Drawing.Size(117, 48);
            this.loginB.TabIndex = 1;
            this.loginB.Text = "Login";
            this.loginB.UseVisualStyleBackColor = true;
            this.loginB.Click += new System.EventHandler(this.loginB_Click);
            // 
            // autoB
            // 
            this.autoB.Location = new System.Drawing.Point(551, 20);
            this.autoB.Margin = new System.Windows.Forms.Padding(4);
            this.autoB.Name = "autoB";
            this.autoB.Size = new System.Drawing.Size(155, 61);
            this.autoB.TabIndex = 4;
            this.autoB.Text = "start";
            this.autoB.UseVisualStyleBackColor = true;
            this.autoB.Click += new System.EventHandler(this.autoB_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "rate";
            // 
            // rate
            // 
            this.rate.Location = new System.Drawing.Point(61, 20);
            this.rate.Margin = new System.Windows.Forms.Padding(4);
            this.rate.Name = "rate";
            this.rate.Size = new System.Drawing.Size(105, 22);
            this.rate.TabIndex = 11;
            this.rate.Text = "1";
            this.rate.Validating += new System.ComponentModel.CancelEventHandler(this.rate_Validating);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(176, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "ms";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F);
            this.label3.Location = new System.Drawing.Point(620, 827);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Copyright  Thomas";
            // 
            // logT
            // 
            this.logT.Location = new System.Drawing.Point(1062, 187);
            this.logT.Margin = new System.Windows.Forms.Padding(4);
            this.logT.Name = "logT";
            this.logT.ReadOnly = true;
            this.logT.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.logT.Size = new System.Drawing.Size(453, 620);
            this.logT.TabIndex = 9;
            this.logT.Text = "";
            this.logT.TextChanged += new System.EventHandler(this.logT_TextChanged);
            // 
            // addB
            // 
            this.addB.Location = new System.Drawing.Point(623, 145);
            this.addB.Margin = new System.Windows.Forms.Padding(4);
            this.addB.Name = "addB";
            this.addB.Size = new System.Drawing.Size(101, 33);
            this.addB.TabIndex = 5;
            this.addB.Text = "import";
            this.addB.UseVisualStyleBackColor = true;
            this.addB.Click += new System.EventHandler(this.addB_Click);
            // 
            // deleteMail
            // 
            this.deleteMail.Location = new System.Drawing.Point(732, 145);
            this.deleteMail.Margin = new System.Windows.Forms.Padding(4);
            this.deleteMail.Name = "deleteMail";
            this.deleteMail.Size = new System.Drawing.Size(101, 33);
            this.deleteMail.TabIndex = 6;
            this.deleteMail.Text = "delete";
            this.deleteMail.UseVisualStyleBackColor = true;
            this.deleteMail.Click += new System.EventHandler(this.deleteB_Click);
            // 
            // inputT
            // 
            this.inputT.Location = new System.Drawing.Point(1209, 81);
            this.inputT.Margin = new System.Windows.Forms.Padding(4);
            this.inputT.Name = "inputT";
            this.inputT.Size = new System.Drawing.Size(239, 22);
            this.inputT.TabIndex = 2;
            this.inputT.Visible = false;
            this.inputT.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_keyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1131, 93);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "username:";
            this.label4.Visible = false;
            // 
            // testLog
            // 
            this.testLog.Location = new System.Drawing.Point(104, 879);
            this.testLog.Margin = new System.Windows.Forms.Padding(4);
            this.testLog.Name = "testLog";
            this.testLog.ReadOnly = true;
            this.testLog.Size = new System.Drawing.Size(1205, 345);
            this.testLog.TabIndex = 16;
            this.testLog.Text = "";
            this.testLog.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(1209, 120);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(239, 22);
            this.textBox1.TabIndex = 3;
            this.textBox1.UseSystemPasswordChar = true;
            this.textBox1.Visible = false;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_keyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1131, 124);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 17);
            this.label5.TabIndex = 14;
            this.label5.Text = "password:";
            this.label5.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(551, 94);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(350, 43);
            this.button1.TabIndex = 18;
            this.button1.Text = "test the mail cracker";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(817, 827);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 17);
            this.label6.TabIndex = 19;
            this.label6.Text = "label6";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(732, 20);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(169, 61);
            this.button2.TabIndex = 20;
            this.button2.Text = "stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(100, 145);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(99, 33);
            this.button3.TabIndex = 5;
            this.button3.Text = "import";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.addDetails_Click);
            // 
            // deleteApp
            // 
            this.deleteApp.Location = new System.Drawing.Point(207, 145);
            this.deleteApp.Margin = new System.Windows.Forms.Padding(4);
            this.deleteApp.Name = "deleteApp";
            this.deleteApp.Size = new System.Drawing.Size(101, 33);
            this.deleteApp.TabIndex = 6;
            this.deleteApp.Text = "delete";
            this.deleteApp.UseVisualStyleBackColor = true;
            this.deleteApp.Click += new System.EventHandler(this.deleteDetails_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 163);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 17);
            this.label7.TabIndex = 21;
            this.label7.Text = "details:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(559, 163);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 17);
            this.label8.TabIndex = 21;
            this.label8.Text = "emails:";
            // 
            // appointmentGrid
            // 
            this.appointmentGrid.AutoGenerateColumns = false;
            this.appointmentGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.appointmentGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cardNoDataGridViewTextBoxColumn,
            this.cardPasswordDataGridViewTextBoxColumn,
            this.chineseNameDataGridViewTextBoxColumn,
            this.japaneseNameDataGridViewTextBoxColumn,
            this.phoneDataGridViewTextBoxColumn});
            this.appointmentGrid.DataSource = this.appointmentBindingSource2;
            this.appointmentGrid.Location = new System.Drawing.Point(27, 187);
            this.appointmentGrid.Margin = new System.Windows.Forms.Padding(4);
            this.appointmentGrid.Name = "appointmentGrid";
            this.appointmentGrid.RowTemplate.Height = 23;
            this.appointmentGrid.Size = new System.Drawing.Size(516, 431);
            this.appointmentGrid.TabIndex = 24;
            this.appointmentGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.appointmentGrid_CellContentClick);
            // 
            // cardNoDataGridViewTextBoxColumn
            // 
            this.cardNoDataGridViewTextBoxColumn.DataPropertyName = "CardNo";
            this.cardNoDataGridViewTextBoxColumn.HeaderText = "CardNo";
            this.cardNoDataGridViewTextBoxColumn.Name = "cardNoDataGridViewTextBoxColumn";
            this.cardNoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cardPasswordDataGridViewTextBoxColumn
            // 
            this.cardPasswordDataGridViewTextBoxColumn.DataPropertyName = "CardPassword";
            this.cardPasswordDataGridViewTextBoxColumn.HeaderText = "CardPassword";
            this.cardPasswordDataGridViewTextBoxColumn.Name = "cardPasswordDataGridViewTextBoxColumn";
            this.cardPasswordDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // chineseNameDataGridViewTextBoxColumn
            // 
            this.chineseNameDataGridViewTextBoxColumn.DataPropertyName = "ChineseName";
            this.chineseNameDataGridViewTextBoxColumn.HeaderText = "ChineseName";
            this.chineseNameDataGridViewTextBoxColumn.Name = "chineseNameDataGridViewTextBoxColumn";
            this.chineseNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // japaneseNameDataGridViewTextBoxColumn
            // 
            this.japaneseNameDataGridViewTextBoxColumn.DataPropertyName = "JapaneseName";
            this.japaneseNameDataGridViewTextBoxColumn.HeaderText = "JapaneseName";
            this.japaneseNameDataGridViewTextBoxColumn.Name = "japaneseNameDataGridViewTextBoxColumn";
            this.japaneseNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // phoneDataGridViewTextBoxColumn
            // 
            this.phoneDataGridViewTextBoxColumn.DataPropertyName = "Phone";
            this.phoneDataGridViewTextBoxColumn.HeaderText = "Phone";
            this.phoneDataGridViewTextBoxColumn.Name = "phoneDataGridViewTextBoxColumn";
            this.phoneDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // appointmentBindingSource2
            // 
            this.appointmentBindingSource2.DataSource = typeof(widkeyPaperDiaper.Client);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(25, 625);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(517, 183);
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label9.Location = new System.Drawing.Point(545, 647);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 25);
            this.label9.TabIndex = 26;
            this.label9.Text = "label9";
            this.label9.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(551, 716);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(132, 22);
            this.textBox2.TabIndex = 27;
            // 
            // mailGrid
            // 
            this.mailGrid.AutoGenerateColumns = false;
            this.mailGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mailGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.emailDataGridViewTextBoxColumn,
            this.passwordDataGridViewTextBoxColumn});
            this.mailGrid.DataSource = this.emailForshowBindingSource;
            this.mailGrid.Location = new System.Drawing.Point(551, 187);
            this.mailGrid.Margin = new System.Windows.Forms.Padding(4);
            this.mailGrid.Name = "mailGrid";
            this.mailGrid.RowTemplate.Height = 23;
            this.mailGrid.Size = new System.Drawing.Size(484, 431);
            this.mailGrid.TabIndex = 24;
            // 
            // emailDataGridViewTextBoxColumn
            // 
            this.emailDataGridViewTextBoxColumn.DataPropertyName = "Email";
            this.emailDataGridViewTextBoxColumn.HeaderText = "Email";
            this.emailDataGridViewTextBoxColumn.Name = "emailDataGridViewTextBoxColumn";
            this.emailDataGridViewTextBoxColumn.ReadOnly = true;
            this.emailDataGridViewTextBoxColumn.Width = 220;
            // 
            // passwordDataGridViewTextBoxColumn
            // 
            this.passwordDataGridViewTextBoxColumn.DataPropertyName = "Password";
            this.passwordDataGridViewTextBoxColumn.HeaderText = "Password";
            this.passwordDataGridViewTextBoxColumn.Name = "passwordDataGridViewTextBoxColumn";
            this.passwordDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // emailForshowBindingSource
            // 
            this.emailForshowBindingSource.DataSource = typeof(widkeyPaperDiaper.Form1.EmailForshow);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "北海道",
            "青森県",
            "宮城県",
            "茨城県",
            "栃木県",
            "群馬県",
            "千葉県",
            "埼玉県",
            "東京都",
            "神奈川県",
            "新潟県",
            "長野県",
            "山梨県",
            "石川県",
            "福井県",
            "静岡県",
            "岐阜県",
            "愛知県",
            "滋賀県",
            "兵庫県",
            "大阪府",
            "奈良県",
            "岡山県",
            "和歌山県",
            "広島県",
            "山口県",
            "愛媛県",
            "福岡県",
            "佐賀県"});
            this.comboBox1.Location = new System.Drawing.Point(300, 20);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(199, 24);
            this.comboBox1.TabIndex = 28;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(300, 61);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(199, 24);
            this.comboBox2.TabIndex = 29;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(240, 31);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 17);
            this.label10.TabIndex = 30;
            this.label10.Text = "county";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(256, 72);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(39, 17);
            this.label11.TabIndex = 31;
            this.label11.Text = "shop";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(16, 72);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 17);
            this.label12.TabIndex = 32;
            this.label12.Text = "type";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "M",
            "L"});
            this.comboBox3.Location = new System.Drawing.Point(61, 61);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(148, 24);
            this.comboBox3.TabIndex = 33;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(256, 120);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 17);
            this.label13.TabIndex = 34;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(253, 92);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 17);
            this.label14.TabIndex = 35;
            // 
            // appointmentBindingSource
            // 
            this.appointmentBindingSource.DataSource = typeof(widkeyPaperDiaper.Client);
            // 
            // appointmentBindingSource1
            // 
            this.appointmentBindingSource1.DataSource = typeof(widkeyPaperDiaper.Client);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1532, 867);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.mailGrid);
            this.Controls.Add(this.appointmentGrid);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.testLog);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.deleteApp);
            this.Controls.Add(this.inputT);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.deleteMail);
            this.Controls.Add(this.addB);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logT);
            this.Controls.Add(this.autoB);
            this.Controls.Add(this.loginB);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "NZwhv ";
            ((System.ComponentModel.ISupportInitialize)(this.appointmentGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mailGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emailForshowBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.appointmentBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loginB;
        private System.Windows.Forms.Button autoB;
        private System.Windows.Forms.Label label1;
        public  System.Windows.Forms.TextBox rate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private  System.Windows.Forms.RichTextBox logT;
        private System.Windows.Forms.Button addB;
        public  System.Windows.Forms.Button deleteMail;
        public  System.Windows.Forms.TextBox inputT;
        private System.Windows.Forms.Label label4;
        private  System.Windows.Forms.RichTextBox testLog;
        public  System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        public System.Windows.Forms.Button deleteApp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.DataGridView appointmentGrid;
        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label9;
        public System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.BindingSource appointmentBindingSource;
        private System.Windows.Forms.BindingSource appointmentBindingSource2;
        private System.Windows.Forms.BindingSource appointmentBindingSource1;
        private System.Windows.Forms.DataGridViewTextBoxColumn cardNoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cardPasswordDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn chineseNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn japaneseNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn phoneDataGridViewTextBoxColumn;
        public System.Windows.Forms.DataGridView mailGrid;
        private System.Windows.Forms.BindingSource emailForshowBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn emailDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn passwordDataGridViewTextBoxColumn;
        public System.Windows.Forms.ComboBox comboBox1;
        public System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label13;
        public System.Windows.Forms.Label label14;
    }
}

