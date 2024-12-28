public partial class MenuForm : Form
{
    private PictureBox pictureBox;
    private Button nextButton;
    private Button prevButton;
    private string[] imagePaths;
    private int currentIndex;

    public MenuForm()
    {
        // تهيئة الحقول هنا
        pictureBox = new PictureBox();
        nextButton = new Button();
        prevButton = new Button();
        imagePaths = new string[]
         {
            "images/WhatsApp Image 2024-12-12 at 16.34.37_7627db62.jpg",
            "images/WhatsApp Image 2024-12-12 at 16.42.42_f4061361.jpg",
            "images/WhatsApp Image 2024-12-12 at 16.42.43_33957a9c.jpg"
        };
        currentIndex = 0;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.pictureBox.Location = new Point(50, 50);
        this.pictureBox.Size = new Size(400, 300);
        this.pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        this.pictureBox.Image = Image.FromFile(imagePaths[currentIndex]);
        this.pictureBox.Click += new EventHandler(this.PictureBox_Click); // إضافة حدث النقر

        this.nextButton.Location = new Point(370, 360);
        this.nextButton.Size = new Size(80, 30);
        this.nextButton.Text = "Sonraki";
        this.nextButton.Click += new EventHandler(this.NextButton_Click);

        this.prevButton.Location = new Point(50, 360);
        this.prevButton.Size = new Size(80, 30);
        this.prevButton.Text = "Önceki";
        this.prevButton.Click += new EventHandler(this.PrevButton_Click);

        this.ClientSize = new Size(500, 400);
        this.Controls.Add(this.pictureBox);
        this.Controls.Add(this.nextButton);
        this.Controls.Add(this.prevButton);
        this.Text = "Menü";
    }

    private void PictureBox_Click(object sender, EventArgs e)
    {
        ZoomForm zoomForm = new ZoomForm(pictureBox.Image);
        zoomForm.Show();
    }

    private void NextButton_Click(object sender, EventArgs e)
    {
        currentIndex = (currentIndex + 1) % imagePaths.Length;
        pictureBox.Image = Image.FromFile(imagePaths[currentIndex]);
    }

    private void PrevButton_Click(object sender, EventArgs e)
    {
        currentIndex = (currentIndex - 1 + imagePaths.Length) % imagePaths.Length;
        pictureBox.Image = Image.FromFile(imagePaths[currentIndex]);
    }
}
