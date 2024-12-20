namespace Calculator
{
    public partial class Form1 : Form
    {
        private readonly CalculatorController _controller;
        public Form1()
        {
            InitializeComponent();
            _controller = new CalculatorController();
        }

        private void buttonClick(object sender, EventArgs e)
        {
            var currentButton = sender as Button;
            txtInput.Text += currentButton.Text;
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text;

            try
            {
                double result = _controller.Calculate(input);
                txtInput.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void ResultCheck_Click(object sender, EventArgs e)
        {

            string input = txtInput.Text;

            try
            {
                double result = _controller.Calculate(input);
                txtInput.Text = result.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = string.Empty;
        }
    }
}
