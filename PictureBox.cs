public class ZoomForm : Form
{
    private PictureBox pictureBox;

    public ZoomForm(Image image)
    {
        InitializeComponent();
        pictureBox.Image = image;
    }

    private void InitializeComponent()
    {
        this.pictureBox = new PictureBox();
        this.pictureBox.Dock = DockStyle.Fill;
        this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

        this.ClientSize = new Size(800, 600);
        this.Controls.Add(this.pictureBox);
        this.Text = "Zoomed Image";
    }
}
