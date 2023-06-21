using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RSAproject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        int p, q, n, phiN, a, b;
        private string RSAplainText,RSAencryptedText;
        /// <summary>
        /// hàm kiểm tra số nguyên tố
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool isPrime(int n)
        {
            if (n <= 1)
            {
                return false;
            }

            for (int i = 2; i <= n / 2; i++)
            {
                if (n % i == 0)
                {
                    return false;
                }
            }

            return true;
        }
        /// <summary>
        /// tìm UCLN của 2 số nguyên ( giải thuật Euclide)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private int gcd(int a,int b)
        {
            //sử dụng giải thuật Euclide
            while (b != 0)
            {
                int r = a % b;
                a = b;
                b = r;
            }
            return a;
        }
        /// <summary>
        /// kiểm tra 2 số nguyên có nguyên tố cùng nhau hay không
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool nguyenToCungNhau(int a, int b)
        {
            if (gcd(a, b) == 1) 
                return true; 
            else 
                return false;
        }
        /// <summary>
        /// hàm tính mod luỹ thừa lớn sử thuật toán bp và nhân
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ex"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public int computeMod(int x, int ex, int n)
        {

            //Sử dụng thuật toán "bình phương nhân"
            //Chuyển e sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = ex % 2;
                k++;
                ex = ex / 2;
            }
            while (ex != 0);
            //Quá trình lấy dư
            int p = 1;
            for (int i = k - 1; i >= 0; i--)
            {
                p = (p * p) % n;
                if (a[i] == 1)
                    p = (p * x) % n;
            }
            return p;
        }
        /// <summary>
        /// hàm tính mod nghịch đảo sử dụng euclide mở rộng
        /// </summary>
        /// <param name="a"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private int modInverse(int a,int m)
        {
            int r,q, t0 = 0, t1 = 1, t=0,temp=m;
            while (a > 0)
            {
                r = m % a;
                if (r == 0) break;
                q = m / a;
                t = t0 - q * t1;
                m = a;
                a = r;
                t0 = t1;
                t1 = t;
            }
            if (a > 1) return -1;
            if (t >= 0) return t;
            else return t + temp;
        }
        /// <summary>
        /// hàm tạo khoá ngẫu nhiên
        /// </summary>
        private void createKey()
        {
           
            Random rd = new Random();
            do
            {
                p = rd.Next(2, 100);
                q = rd.Next(2, 100);
            } while (!isPrime(p) || !isPrime(q));
            n = p * q;
            phiN = (p - 1) * (q - 1);
            //Tính b là một số ngẫu nhiên có giá trị 0< e <phi(n) và là số nguyên tố cùng nhau với Phi(n)
            do
            {
               b = rd.Next(2, phiN);
            }
            while (!nguyenToCungNhau(b, phiN));
            //Tính a là nghịch đảo modular của b
            a=modInverse(b, phiN);
            return;
        }
        /// <summary>
        /// hàm mã hoá rsa
        /// </summary>
        /// <param name="ChuoiVao"></param>
        public void encryptData(string ChuoiVao)
        {
            // taoKhoa();
            // Chuyen xau thanh ma Unicode
            byte[] temp1 = Encoding.Unicode.GetBytes(ChuoiVao);
            string base64 = Convert.ToBase64String(temp1);

            // Chuyen xau thanh ma Unicode
            int[] temp2 = new int[base64.Length];
            for (int i = 0; i < base64.Length; i++)
            {
                temp2[i] = (int)base64[i];
            }

            //Mảng chứa các kí tự đã mã hóa
            int[] temp3 = new int[temp2.Length];
            for (int i = 0; i < temp2.Length; i++)
            {
                temp3[i] = computeMod(temp2[i], b, n); // mã hóa
            }

            //Chuyển sang kiểu kí tự trong bảng mã Unicode
            string str = "";
            for (int i = 0; i < temp3.Length; i++)
            {
                str = str + (char)temp3[i];
            }
            byte[] data = Encoding.Unicode.GetBytes(str);
            cipherText.Text = Convert.ToBase64String(data);
            //encryptedText.Text = Convert.ToBase64String(data);

        }
        /// <summary>
        /// hàm giải mã rsa
        /// </summary>
        /// <param name="ChuoiVao"></param>
        public void decryptData(string ChuoiVao)
        {
            byte[] temp2 = Convert.FromBase64String(ChuoiVao);
            string giaima = Encoding.Unicode.GetString(temp2);

            int[] k = new int[giaima.Length];
            for (int i = 0; i < giaima.Length; i++)
            {
                k[i] = (int)giaima[i];
            }
            //Giải mã
            int[] c = new int[k.Length];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = computeMod(k[i], a, n);// giải mã
            }

            string str = "";
            for (int i = 0; i < c.Length; i++)
            {
                str = str + (char)c[i];
            }
            byte[] data2 = Convert.FromBase64String(str);
            decryptedText.Text = Encoding.Unicode.GetString(data2);

        }
        private void selectFileBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Plain Text | *.TXT;*.doc;*docx";
                openFile.Title = "Vui lòng chọn bản rõ";
                if(openFile.ShowDialog() == true)
                {
                    if (openFile.FileName != string.Empty)
                    {
                        using (StreamReader sr = new StreamReader(openFile.FileName))
                        {
                            RSAplainText = sr.ReadToEnd();
                            sr.Close();
                        }
                        plainText.Text = RSAplainText;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn đúng định dạng file", "Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi chọn file! "+ex.Message, "Hệ Thống", MessageBoxButton.OK,MessageBoxImage.Information);
                throw;
            }
        }

        private void encryptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                createKey();
                string inputData = plainText.Text;
                encryptData(inputData);
            }
            catch
            {
                MessageBox.Show("Có lỗi trong quá trình mã hoá!","Hệ thống",MessageBoxButton.OK,MessageBoxImage.Error);
                throw;
            }
        }

        private void selectEncryptedFilebtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Filter = "Plain Text .TXT|*.TXT";
                openFile.Title = "Vui lòng chọn bản mã";
                if (openFile.ShowDialog() == true)
                {
                    if (openFile.FileName != string.Empty)
                    {
                        using (StreamReader sr = new StreamReader(openFile.FileName))
                        {
                            RSAencryptedText = sr.ReadToEnd();
                            sr.Close();
                        }
                        encryptedText.Text = RSAencryptedText;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn đúng định dạng file", "Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

            }
            catch
            {
                MessageBox.Show("Lỗi chọn file!", "Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Information);
                throw;
            }
        }

        private void decryptBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputData = encryptedText.Text;
                decryptData(inputData);
            }
            catch
            {
                MessageBox.Show("Có lỗi trong quá trình giải mã!", "Hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void saveFilebtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result;
            result = MessageBox.Show("Bạn có muốn lưu file?", "Hệ thống", MessageBoxButton.YesNo, MessageBoxImage.Question); if (result == MessageBoxResult.No)
            {
                return;
            }
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (cipherText.Text != null )
                    {
                        SaveFileDialog saveFile = new SaveFileDialog();
                        saveFile.FileName = "CipherText.txt";
                        saveFile.Filter = "Text files | *.txt";
                        if (saveFile.ShowDialog() == true)
                        {
                            File.WriteAllText(saveFile.FileName,cipherText.Text );
                        }
                        else
                        {
                            MessageBox.Show("Lỗi trong quá trình lưu file!", "Hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }

        }

        private void sendTextbtn_Click(object sender, RoutedEventArgs e)
        {
            string sendText = cipherText.Text;
            encryptedText.Text = sendText;
        }

    }
}
