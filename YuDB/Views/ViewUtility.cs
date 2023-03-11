using System.Text;

namespace YuDB.Views
{
    public class ViewUtility
    {
        public static string ContainWithinRectangle(int width, params string[] strings)
        {
            var result = new StringBuilder("╔");
            for (int i = 0; i < width; i++)
                result.Append("═");
            result.Append("╗\n");

            foreach (var str in strings)
            {
                if (str.Equals("=="))
                {
                    result.Append("╠");
                    for (int i = 0; i < width; i++)
                        result.Append("═");
                    result.Append("╣\n");
                }
                else if (str.Equals("--"))
                {
                    result.Append("╟");
                    for (int i = 0; i < width; i++)
                        result.Append("─");
                    result.Append("╢\n");
                }
                else
                {
                    result.Append("║");
                    result.Append(string.Format(string.Format("{{0, -{0}}}", width), str));
                    result.Append("║\n");
                }
            }

            result.Append("╚");
            for (int i = 0; i < width; i++)
                result.Append("═");
            result.Append('╝');

            return result.ToString();
        }
    }
}