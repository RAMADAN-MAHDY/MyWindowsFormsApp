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

    public AdminForm()
    {
        InitializeComponent();
        LoadConfiguration();
        SetConnection();
    }

    private void InitializeComponent()
    {
        this.txtTableNumber = new System.Windows.Forms.TextBox();
        this.txtCapacity = new System.Windows.Forms.TextBox();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.addButton = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // txtTableNumber
        // 
        this.txtTableNumber.Location = new System.Drawing.Point(12, 12);
        this.txtTableNumber.Name = "txtTableNumber";
        this.txtTableNumber.Size = new System.Drawing.Size(100, 20);
        this.txtTableNumber.TabIndex = 0;
        this.txtTableNumber.PlaceholderText = "Table Number";
        // 
        // txtCapacity
        // 
        this.txtCapacity.Location = new System.Drawing.Point(12, 38);
        this.txtCapacity.Name = "txtCapacity";
        this.txtCapacity.Size = new System.Drawing.Size(100, 20);
        this.txtCapacity.TabIndex = 1;
        this.txtCapacity.PlaceholderText = "Capacity";
        // 
        // txtPassword
        // 
        this.txtPassword.Location = new System.Drawing.Point(12, 64);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.Size = new System.Drawing.Size(100, 20);
        this.txtPassword.TabIndex = 2;
        this.txtPassword.PlaceholderText = "Password";
        // 
        // addButton
        // 
        this.addButton.Location = new System.Drawing.Point(12, 90);
        this.addButton.Name = "addButton";
        this.addButton.Size = new System.Drawing.Size(75, 23);
        this.addButton.TabIndex = 3;
        this.addButton.Text = "Add Table";
        this.addButton.UseVisualStyleBackColor = true;
        this.addButton.Click += new System.EventHandler(this.addButton_Click);
        // 
        // AdminForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(284, 261);
        this.Controls.Add(this.addButton);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.txtCapacity);
        this.Controls.Add(this.txtTableNumber);
        this.Name = "AdminForm";
        this.Text = "AdminForm";
        this.ResumeLayout(false);
        this.PerformLayout();
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

    private void addButton_Click(object? sender, EventArgs e)
    {
        if (txtPassword!.Text != "123456")
        {
            MessageBox.Show("Incorrect password!");
            return;
        }

        int tableNumber = int.Parse(txtTableNumber!.Text);
        int capacity = int.Parse(txtCapacity!.Text);
        string query = $"INSERT INTO tables (table_number, capacity) VALUES ({tableNumber}, {capacity});";
        ExecuteQuery(query);

        MessageBox.Show("Table added successfully!");
    }
}
