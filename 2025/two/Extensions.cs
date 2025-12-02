using System.Text;

public static class Extensions
{
    extension(string str)
    {
        public string Repeat(int n)
        {
            var strBuilder = new StringBuilder();
            for (var i = 0; i < n; i++)
            {
                strBuilder.Append(str);
            }
            return strBuilder.ToString();
        }
    }
}