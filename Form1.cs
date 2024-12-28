using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyWindowsFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
    
            Label titleLabel = new Label();
            titleLabel.Text = "Masa Rezervasyon ve Rezervasyon Yönetim Sistemi";
            titleLabel.Font = new Font("Segoe UI", 30, FontStyle.Bold); 
            titleLabel.ForeColor = Color.White;  
            titleLabel.BackColor = Color.Transparent;  
            titleLabel.Location = new Point(120, 300); 
            titleLabel.AutoSize = true;

           
            titleLabel.Paint += (sender, e) =>
            {
                
                e.Graphics.DrawString(titleLabel.Text, titleLabel.Font, Brushes.Gray, new PointF(titleLabel.Location.X + 5, titleLabel.Location.Y + 5));
            
                e.Graphics.DrawString(titleLabel.Text, titleLabel.Font, Brushes.White, new PointF(titleLabel.Location.X, titleLabel.Location.Y));
            };

           
            this.Controls.Add(titleLabel);

        
            this.BackColor = Color.FromArgb(40, 40, 40);  
        }

        private void AdminButton_Click(object? sender, EventArgs e)
        {
            AdminForm adminForm = new AdminForm();
            adminForm.Show();
        }

        private void CustomerButton_Click(object? sender, EventArgs e)
        {
            CustomerForm customerForm = new CustomerForm();
            customerForm.Show();
        }

        private void MenuButton_Click(object? sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();
        }
    }
}
