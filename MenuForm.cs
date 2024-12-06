using System;
using System.Windows.Forms;

public partial class MenuForm : Form
{
    public MenuForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // MenuForm
        // 
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Name = "MenuForm";
        this.Text = "Menu";
        this.ResumeLayout(false);
    }
}
