using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using System.IO;

public partial class CustomerForm : Form
{
    private SqlConnection? sql_con;
    private SqlDataAdapter? dataAdapter;
    private DataTable? dataTable;
    private IConfiguration? configuration;
    private DataGridView? tablesGridView;
    private TextBox? txtTableNumber;
    private TextBox? txtCustomerName;
    private DateTimePicker? dateTimePicker;
    private TextBox? txtDuration;
    private Button? reserveButton;
    private Label? lblTableNumber;
    private Label? lblCustomerName;
    private Label? lblReservationTime;
    private Label? lblDuration;

    public CustomerForm()
    {
        InitializeComponent();
        LoadConfiguration();
        SetConnection();
        LoadTables();
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
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    configuration = builder.Build();

    string connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
    sql_con = new SqlConnection(connectionString);
}



private void LoadTables()
{
    if (sql_con?.State == ConnectionState.Closed)
        sql_con?.Open();
    string query = "SELECT table_number, capacity FROM tables"; // إزالة عمود is_reserved
    dataAdapter = new SqlDataAdapter(query, sql_con);
    dataTable = new DataTable();
    dataAdapter.Fill(dataTable);
    tablesGridView!.DataSource = dataTable;

    sql_con?.Close();
}


private void ReserveTable(int tableNumber, string customerName, DateTime reservationTime, int duration)
{
    string getTableIdQuery = $"SELECT id FROM tables WHERE table_number = @tableNumber";
    int tableId = GetTableId(getTableIdQuery, tableNumber);

    string checkConflictQuery = @"
        SELECT COUNT(*)
        FROM reservations
        WHERE table_id = @tableId AND (
            @reservationTime < DATEADD(MINUTE, duration, reservation_time) AND
            DATEADD(MINUTE, @duration, @reservationTime) > reservation_time
        )";
    SqlCommand checkCommand = new SqlCommand(checkConflictQuery, sql_con);
    checkCommand.Parameters.AddWithValue("@tableId", tableId);
    checkCommand.Parameters.AddWithValue("@reservationTime", reservationTime);
    checkCommand.Parameters.AddWithValue("@duration", duration);

    sql_con?.Open();
    int conflictCount = (int)checkCommand.ExecuteScalar()!;
    sql_con?.Close();

    if (conflictCount > 0)
    {
        MessageBox.Show("The reservation conflicts with an existing reservation. Please choose a different time or duration.");
        return;
    }

    string query = @"
        INSERT INTO reservations (table_id, customer_name, reservation_time, duration)
        VALUES (@tableId, @customerName, @reservationTime, @duration)";
    SqlCommand insertCommand = new SqlCommand(query, sql_con);
    insertCommand.Parameters.AddWithValue("@tableId", tableId);
    insertCommand.Parameters.AddWithValue("@customerName", customerName);
    insertCommand.Parameters.AddWithValue("@reservationTime", reservationTime);
    insertCommand.Parameters.AddWithValue("@duration", duration);
    ExecuteQuery(insertCommand);

    query = $"UPDATE tables SET is_reserved = 1 WHERE table_number = @tableNumber";
    SqlCommand updateCommand = new SqlCommand(query, sql_con);
    updateCommand.Parameters.AddWithValue("@tableNumber", tableNumber);
    ExecuteQuery(updateCommand);

    var timer = new System.Timers.Timer(duration * 60 * 1000);
    timer.Elapsed += (sender, e) =>
    {
        string resetQuery = $"UPDATE tables SET is_reserved = 0 WHERE table_number = @tableNumber";
        SqlCommand resetCommand = new SqlCommand(resetQuery, sql_con);
        resetCommand.Parameters.AddWithValue("@tableNumber", tableNumber);
        ExecuteQuery(resetCommand);
        timer.Stop();
    };
    timer.Start();

    MessageBox.Show("تم الحجز بنجاح!");
}


private int GetTableId(string query, int tableNumber)
{
    if (sql_con?.State == ConnectionState.Closed)
        sql_con?.Open();
    SqlCommand sql_cmd = new SqlCommand(query, sql_con);
    sql_cmd.Parameters.AddWithValue("@tableNumber", tableNumber);
    int tableId = (int)sql_cmd.ExecuteScalar();
    sql_con?.Close();
    return tableId;
}

private void ExecuteQuery(SqlCommand sql_cmd)
{
    if (sql_con?.State == ConnectionState.Closed)
        sql_con?.Open();
    sql_cmd.ExecuteNonQuery();
    sql_con?.Close();
}


private void reserveButton_Click(object? sender, EventArgs e)
{
    if (string.IsNullOrWhiteSpace(txtTableNumber!.Text) ||
        string.IsNullOrWhiteSpace(txtCustomerName!.Text) ||
        string.IsNullOrWhiteSpace(txtDuration!.Text))
    {
        MessageBox.Show("يرجى ملء جميع الحقول المطلوبة.");
        return;
    }

    int tableNumber = int.Parse(txtTableNumber!.Text);
    string customerName = txtCustomerName!.Text;
    DateTime reservationTime = dateTimePicker!.Value;
    int duration = int.Parse(txtDuration!.Text);

    ReserveTable(tableNumber, customerName, reservationTime, duration);
}



    private void tablesGridView_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            DataGridViewRow row = tablesGridView!.Rows[e.RowIndex];
            txtTableNumber!.Text = row.Cells["table_number"].Value.ToString();
            dateTimePicker!.Value = DateTime.Now; // Reset to current time for a new reservation
            txtDuration!.Text = "60"; // Default duration
        }
    }



        private void InitializeComponent()
    {
        this.tablesGridView = new System.Windows.Forms.DataGridView();
        this.txtTableNumber = new System.Windows.Forms.TextBox();
        this.txtCustomerName = new System.Windows.Forms.TextBox();
        this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
        this.txtDuration = new System.Windows.Forms.TextBox();
        this.reserveButton = new System.Windows.Forms.Button();
        this.lblTableNumber = new System.Windows.Forms.Label();
        this.lblCustomerName = new System.Windows.Forms.Label();
        this.lblReservationTime = new System.Windows.Forms.Label();
        this.lblDuration = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.tablesGridView)).BeginInit();
        this.SuspendLayout();
        
        // 
        // tablesGridView
        // 
        this.tablesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.tablesGridView.Location = new System.Drawing.Point(12, 12);
        this.tablesGridView.Name = "tablesGridView";
        this.tablesGridView.Size = new System.Drawing.Size(276, 150);
        this.tablesGridView.TabIndex = 0;
        this.tablesGridView.CellClick += new DataGridViewCellEventHandler(this.tablesGridView_CellClick);
        
        // 
        // lblTableNumber
        // 
        this.lblTableNumber.AutoSize = true;
        this.lblTableNumber.Location = new System.Drawing.Point(12, 170);
        this.lblTableNumber.Name = "lblTableNumber";
        this.lblTableNumber.Size = new System.Drawing.Size(174, 13);
        this.lblTableNumber.TabIndex = 6;
        this.lblTableNumber.Text = "Table Number:";
        
        // 
        // txtTableNumber
        // 
        this.txtTableNumber.Location = new System.Drawing.Point(12, 190);
        this.txtTableNumber.Name = "txtTableNumber";
        this.txtTableNumber.Size = new System.Drawing.Size(100, 20);
        this.txtTableNumber.TabIndex = 1;
        
        // 
        // lblCustomerName
        // 
        this.lblCustomerName.AutoSize = true;
        this.lblCustomerName.Location = new System.Drawing.Point(118, 170);
        this.lblCustomerName.Name = "lblCustomerName";
        this.lblCustomerName.Size = new System.Drawing.Size(85, 13);
        this.lblCustomerName.TabIndex = 7;
        this.lblCustomerName.Text = "Customer Name:";
        
        // 
        // txtCustomerName
        // 
        this.txtCustomerName.Location = new System.Drawing.Point(118, 190);
        this.txtCustomerName.Name = "txtCustomerName";
        this.txtCustomerName.Size = new System.Drawing.Size(150, 20);
        this.txtCustomerName.TabIndex = 2;
        
        // 
        // lblReservationTime
        // 
        this.lblReservationTime.AutoSize = true;
        this.lblReservationTime.Location = new System.Drawing.Point(274, 170);
        this.lblReservationTime.Name = "lblReservationTime";
        this.lblReservationTime.Size = new System.Drawing.Size(96, 13);
        this.lblReservationTime.TabIndex = 8;
        this.lblReservationTime.Text = "Reservation Time:";
        
        // 
        // dateTimePicker
        // 
       this.dateTimePicker.CustomFormat = "yyyy-MM-dd HH:mm";
this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
this.dateTimePicker.Location = new System.Drawing.Point(274, 190);
this.dateTimePicker.Name = "dateTimePicker";
this.dateTimePicker.Size = new System.Drawing.Size(200, 20);
this.dateTimePicker.TabIndex = 3;

        
        // 
        // lblDuration
        // 
        this.lblDuration.AutoSize = true;
        this.lblDuration.Location = new System.Drawing.Point(480, 170);
        this.lblDuration.Name = "lblDuration";
        this.lblDuration.Size = new System.Drawing.Size(50, 13);
        this.lblDuration.TabIndex = 9;
        this.lblDuration.Text = "Set duration in minutes:";
        
        // 
        // txtDuration
        // 
        this.txtDuration.Location = new System.Drawing.Point(480, 190);
        this.txtDuration.Name = "txtDuration";
        this.txtDuration.Size = new System.Drawing.Size(100, 20);
        this.txtDuration.TabIndex = 4;
        
        // 
        // reserveButton
        // 
        this.reserveButton.BackColor = System.Drawing.Color.LightGreen;
        this.reserveButton.Location = new System.Drawing.Point(586, 187);
        this.reserveButton.Name = "reserveButton";
        this.reserveButton.Size = new System.Drawing.Size(75, 23);
        this.reserveButton.TabIndex = 5;
        this.reserveButton.Text = "Reserve";
        this.reserveButton.UseVisualStyleBackColor = false;
        this.reserveButton.Click += new System.EventHandler(this.reserveButton_Click);
        
        // 
        // CustomerForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.lblDuration);
        this.Controls.Add(this.lblReservationTime);
        this.Controls.Add(this.lblCustomerName);
        this.Controls.Add(this.lblTableNumber);
        this.Controls.Add(this.reserveButton);
        this.Controls.Add(this.txtDuration);
        this.Controls.Add(this.dateTimePicker);
        this.Controls.Add(this.txtCustomerName);
        this.Controls.Add(this.txtTableNumber);
        this.Controls.Add(this.tablesGridView);
        this.Name = "CustomerForm";
        this.Text = "CustomerForm";
        ((System.ComponentModel.ISupportInitialize)(this.tablesGridView)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
