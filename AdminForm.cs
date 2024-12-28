using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using System.IO;

public partial class AdminForm : Form
{
    private SqlConnection? sql_con;
    private SqlDataAdapter? dataAdapter;
    private DataTable? dataTable;
    private IConfiguration? configuration;
    private TextBox? txtTableNumber;
    private TextBox? txtCapacity;
    private TextBox? txtPassword;
    private Button? addButton;
    private Button? editButton;
    private Button? deleteButton;
    private Button? cancelButton;
    private Button? viewReservedButton;
    private Button? loginButton;
    private Label? lblName;
    private Label? lblCapacity;
    private Panel reservationsPanel;
    private DataGridView reservationsGridView;

    public AdminForm()
    {
        InitializeComponent();
        LoadConfiguration();
        SetConnection();
    }

    private void LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration = builder.Build();
    }

    private void SetConnection()
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        sql_con = new SqlConnection(connectionString);
    }

    private void ExecuteQuery(string query)
    {
        if (sql_con?.State == ConnectionState.Closed)
            sql_con?.Open();
        SqlCommand sql_cmd = new SqlCommand(query, sql_con!);
        sql_cmd.ExecuteNonQuery();
        sql_con?.Close();
    }

    private void ToggleAdminControls(bool show)
    {
        txtTableNumber.Visible = show;
        txtCapacity.Visible = show;
        addButton.Visible = show;
        editButton.Visible = show;
        deleteButton.Visible = show;
        cancelButton.Visible = show;
        viewReservedButton.Visible = show;
        lblCapacity.Visible = show;
        lblName.Visible = show;
    }

    private void loginButton_Click(object? sender, EventArgs e)
    {
        if (txtPassword!.Text != "123456")
        {
            MessageBox.Show("Şifre yanlış!");
            return;
        }

        ToggleAdminControls(true); // إظهار الأزرار والحقول بعد التحقق من كلمة المرور
        loginButton.Visible = false; // إخفاء زر تسجيل الدخول بعد التحقق من كلمة المرور
        txtPassword.Visible = false; // إخفاء حقل كلمة المرور بعد التحقق من كلمة المرور
    }

    private void addButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTableNumber.Text))
        {
            MessageBox.Show("Lütfen masa numarasını giriniz.");
            return;
        }

        int tableNumber = int.Parse(txtTableNumber.Text);
        int capacity = int.Parse(txtCapacity.Text);

        // التحقق مما إذا كانت الطاولة موجودة بالفعل
        string checkTableQuery = $"SELECT COUNT(*) FROM tables WHERE table_number = @tableNumber";
        SqlCommand checkCommand = new SqlCommand(checkTableQuery, sql_con);
        checkCommand.Parameters.AddWithValue("@tableNumber", tableNumber);

        sql_con?.Open();
        int tableCount = (int)checkCommand.ExecuteScalar()!;
        sql_con?.Close();

        if (tableCount > 0)
        {
            MessageBox.Show("Masa zaten mevcut. Lütfen farklı bir masa numarası giriniz.");
            return;
        }

        string query = $"INSERT INTO tables (table_number, capacity) VALUES (@tableNumber, @capacity)";
        SqlCommand insertCommand = new SqlCommand(query, sql_con);
        insertCommand.Parameters.AddWithValue("@tableNumber", tableNumber);
        insertCommand.Parameters.AddWithValue("@capacity", capacity);
        ExecuteQuery(insertCommand);

        MessageBox.Show("Masa başarıyla eklendi!");
    }


    private void ExecuteQuery(SqlCommand sql_cmd)
    {
        if (sql_con?.State == ConnectionState.Closed)
            sql_con?.Open();
        sql_cmd.ExecuteNonQuery();
        sql_con?.Close();
    }


    private void editButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTableNumber.Text))
        {
            MessageBox.Show("Lütfen masa numarasını giriniz.");
            return;
        }

        int tableNumber = int.Parse(txtTableNumber.Text);
        int capacity = int.Parse(txtCapacity.Text);
        string query = $"UPDATE tables SET capacity = {capacity} WHERE table_number = {tableNumber};";
        ExecuteQuery(query);

        MessageBox.Show("Masa başarıyla güncellendi!");
    }

    private void deleteButton_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtTableNumber.Text))
        {
            MessageBox.Show("Lütfen masa numarasını giriniz.");
            return;
        }

        if (txtPassword.Text != "123456")
        {
            MessageBox.Show("Şifre yanlış!");
            return;
        }

        int tableNumber = int.Parse(txtTableNumber.Text);
        string checkReservationQuery = $"SELECT is_reserved FROM tables WHERE table_number = {tableNumber};";
        SqlCommand checkCommand = new SqlCommand(checkReservationQuery, sql_con!);

        sql_con?.Open();
        bool isReserved = (bool)checkCommand.ExecuteScalar()!;
        sql_con?.Close();

        if (isReserved)
        {
            MessageBox.Show("Masa rezerve edilmiş. Lütfen önce rezervasyonu iptal edin."); // text
            return;
        }

        string deleteTableQuery = $"DELETE FROM tables WHERE table_number = {tableNumber};";
        ExecuteQuery(deleteTableQuery); // حذف الطاولة

        MessageBox.Show("Masa başarıyla silindi!");
    }

    private void viewReservedButton_Click(object? sender, EventArgs e)
    {
        string query = @"
            SELECT  r.id, t.table_number, t.capacity, r.customer_name, r.reservation_time, r.duration

        try
        {
            string query = @"
            SELECT r.id, t.table_number, t.capacity, r.customer_name, r.reservation_time, r.duration
            FROM tables t
            JOIN reservations r ON t.id = r.table_id
            WHERE t.is_reserved = 1";
            if (sql_con?.State == ConnectionState.Closed)
                sql_con?.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(query, sql_con!);
            DataTable reservedTables = new DataTable();
            adapter.Fill(reservedTables);

            if (!reservedTables.Columns.Contains("Cancel"))
            {
                reservedTables.Columns.Add("Cancel", typeof(string));
                foreach (DataRow row in reservedTables.Rows)
                {
                    row["Cancel"] = "Cancel";
                }
            }

            reservationsGridView!.DataSource = reservedTables; // التأكد من أن reservationsGridView ليس فارغًا

            reservationsGridView.Columns["id"].HeaderText = "Kimlik"; 
            reservationsGridView.Columns["table_number"].HeaderText = "Masa Numarası";
            reservationsGridView.Columns["capacity"].HeaderText = "Kapasite"; 
            reservationsGridView.Columns["customer_name"].HeaderText = "Müşteri Adı";
            reservationsGridView.Columns["reservation_time"].HeaderText = "Rezervasyon Zamanı";
            reservationsGridView.Columns["duration"].HeaderText = "Süre"; 
            reservationsGridView.Columns["Cancel"].HeaderText = "İptal Et";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Rezervasyonlar yüklenirken hata oluştu: {ex.Message}");
        }
        finally
        {
            sql_con?.Close();
        }
    }

    private void reservationsGridView_CellContentClick(object? sender, DataGridViewCellEventArgs e)
    {
        try
        {
            if (reservationsGridView!.Columns["Cancel"] != null && e.ColumnIndex == reservationsGridView.Columns["Cancel"].Index && e.RowIndex >= 0)
            {
                if (reservationsGridView.Rows[e.RowIndex].Cells["id"].Value != null)
                {
                    int reservationId = (int)reservationsGridView.Rows[e.RowIndex].Cells["id"].Value;
                    CancelReservation(reservationId);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Rezervasyon iptal edilirken hata oluştu: {ex.Message}");
        }
    }

    private void CancelReservation(int reservationId)
    {
        try
        {
            string query = $"DELETE FROM reservations WHERE id = @reservationId";
            SqlCommand sql_cmd = new SqlCommand(query, sql_con);
            sql_cmd.Parameters.AddWithValue("@reservationId", reservationId);
            ExecuteQuery(sql_cmd);

            MessageBox.Show("Rezervasyon başarıyla iptal edildi!");

            // إعادة تحميل بيانات DataGridView بعد الإلغاء
            RefreshReservations();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Rezervasyon iptal edilirken hata oluştu: {ex.Message}");
        }
    }

    private void RefreshReservations()
    {
        try
        {
            if (viewReservedButton != null) // التأكد من عدم وجود null في viewReservedButton
            {
                viewReservedButton_Click(viewReservedButton, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Rezervasyonlar yenilenirken hata oluştu: {ex.Message}");
        }
    }



    private void InitializeComponent()
    {
        this.txtTableNumber = new System.Windows.Forms.TextBox();
        this.txtCapacity = new System.Windows.Forms.TextBox();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.addButton = new System.Windows.Forms.Button();
        this.editButton = new System.Windows.Forms.Button();
        this.deleteButton = new System.Windows.Forms.Button();
        this.cancelButton = new System.Windows.Forms.Button();
        this.viewReservedButton = new System.Windows.Forms.Button();
        this.loginButton = new System.Windows.Forms.Button();
        this.lblName = new System.Windows.Forms.Label();
        this.lblCapacity = new System.Windows.Forms.Label();
        this.reservationsPanel = new System.Windows.Forms.Panel();
        this.reservationsGridView = new System.Windows.Forms.DataGridView();
        ((System.ComponentModel.ISupportInitialize)(this.reservationsGridView)).BeginInit();
        this.reservationsPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // txtTableNumber
        // 
        this.txtTableNumber.Location = new System.Drawing.Point(122, 38);
        this.txtTableNumber.Name = "txtTableNumber";
        this.txtTableNumber.Size = new System.Drawing.Size(100, 20);
        this.txtTableNumber.TabIndex = 0;
        this.txtTableNumber.PlaceholderText = "Masa Numarası";
        this.txtTableNumber.Visible = false;
        // 
        // lblName
        // 
        this.lblName.Text = "Masa Numarası:";
        this.lblName.Location = new System.Drawing.Point(122, 12);
        this.lblName.Visible = false;
        // 
        // lblCapacity
        // 
        this.lblCapacity.Text = "Kapasite:";
        this.lblCapacity.Location = new System.Drawing.Point(12, 12);
        this.lblCapacity.Visible = false;
        // 
        // txtCapacity
        // 
        this.txtCapacity.Location = new System.Drawing.Point(12, 38);
        this.txtCapacity.Name = "txtCapacity";
        this.txtCapacity.Size = new System.Drawing.Size(100, 20);
        this.txtCapacity.TabIndex = 1;
        this.txtCapacity.PlaceholderText = "Kapasite";
        this.txtCapacity.Visible = false;
        // 
        // txtPassword
        // 
        this.txtPassword.Location = new System.Drawing.Point(12, 64);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.Size = new System.Drawing.Size(100, 20);
        this.txtPassword.TabIndex = 2;
        this.txtPassword.PlaceholderText = "Şifre";
        // 
        // addButton
        // 
        this.addButton.Location = new System.Drawing.Point(12, 90);
        this.addButton.Name = "addButton";
        this.addButton.Size = new System.Drawing.Size(75, 23);
        this.addButton.TabIndex = 3;
        this.addButton.Text = "Masa Ekle";
        this.addButton.UseVisualStyleBackColor = true;
        this.addButton.Visible = false;
        this.addButton.Click += new System.EventHandler(this.addButton_Click);
        // 
        // editButton
        // 
        this.editButton.Location = new System.Drawing.Point(12, 120);
        this.editButton.Name = "editButton";
        this.editButton.Size = new System.Drawing.Size(75, 23);
        this.editButton.TabIndex = 4;
        this.editButton.Text = "Masa Düzenle";
        this.editButton.UseVisualStyleBackColor = true;
        this.editButton.Visible = false;
        this.editButton.Click += new System.EventHandler(this.editButton_Click);
        // 
        // deleteButton
        // 
        this.deleteButton.Location = new System.Drawing.Point(12, 150);
        this.deleteButton.Name = "deleteButton";
        this.deleteButton.Size = new System.Drawing.Size(75, 23);
        this.deleteButton.TabIndex = 5;
        this.deleteButton.Text = "Masa Sil";
        this.deleteButton.UseVisualStyleBackColor = true;
        this.deleteButton.Visible = false;
        this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
        // 
        // viewReservedButton
        // 
        this.viewReservedButton.Location = new System.Drawing.Point(12, 210);
        this.viewReservedButton.Name = "viewReservedButton";
        this.viewReservedButton.Size = new System.Drawing.Size(100, 23);
        this.viewReservedButton.TabIndex = 7;
        this.viewReservedButton.Text = "Rezerveyi Görüntüle";
        this.viewReservedButton.UseVisualStyleBackColor = true;
        this.viewReservedButton.Visible = false;
        this.viewReservedButton.Click += new System.EventHandler(this.viewReservedButton_Click);
        // 
        // loginButton
        // 
        this.loginButton.Location = new System.Drawing.Point(130, 62);
        this.loginButton.Name = "loginButton";
        this.loginButton.Size = new System.Drawing.Size(75, 23);
        this.loginButton.TabIndex = 8;
        this.loginButton.Text = "Giriş Yap";
        this.loginButton.UseVisualStyleBackColor = true;
        this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
        // 
        // reservationsPanel
        // 
        this.reservationsPanel.Controls.Add(this.reservationsGridView);
        this.reservationsPanel.Location = new System.Drawing.Point(12, 240);
        this.reservationsPanel.Name = "reservationsPanel";
        this.reservationsPanel.Size = new System.Drawing.Size(600, 200);
        this.reservationsPanel.TabIndex = 9;
        // 
        // reservationsGridView
        // 
        this.reservationsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.reservationsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
        this.reservationsGridView.Location = new System.Drawing.Point(0, 0);
        this.reservationsGridView.Name = "reservationsGridView";
        this.reservationsGridView.Size = new System.Drawing.Size(600, 200);
        this.reservationsGridView.TabIndex = 0;
        this.reservationsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.reservationsGridView_CellContentClick);
        // 
        // AdminForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 561);
        this.Controls.Add(this.reservationsPanel);
        this.Controls.Add(this.loginButton);
        this.Controls.Add(this.lblName);
        this.Controls.Add(this.lblCapacity);
        this.Controls.Add(this.viewReservedButton);
        this.Controls.Add(this.cancelButton);
        this.Controls.Add(this.deleteButton);
        this.Controls.Add(this.editButton);
        this.Controls.Add(this.addButton);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.txtCapacity);
        this.Controls.Add(this.txtTableNumber);
        this.Name = "AdminForm";
        this.Text = "YöneticiFormu";
        this.ResumeLayout(false);
        this.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.reservationsGridView)).EndInit();
        this.reservationsPanel.ResumeLayout(false);
    }

}
