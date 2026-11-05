using System.Globalization;
using System.Text;
namespace Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
public sealed class CsvRepository_KIA
{
    private static readonly string[] Header={"Номер заказа","Фамилия","Имя","Отчество","Счёт","Адрес","Телефон","Дата исполнения","Товар","Цена","Количество","Статус"};
    private static string Escape(string value)=>"\""+value.Replace("\"","\"\"")+"\"";
    public void Save(string path,IEnumerable<Order_KIA> orders)
    {
        string fullPath=Path.GetFullPath(path),temporary=fullPath+"."+Guid.NewGuid().ToString("N")+".tmp";
        try
        {
            using(var writer=new StreamWriter(temporary,false,new UTF8Encoding(true)))
            {
                writer.WriteLine(string.Join(";",Header.Select(Escape)));
                foreach(var o in orders)
                {
                    string[] row={o.Number,o.LastName,o.FirstName,o.Patronymic,o.Account,o.Address,o.Phone,o.DueDate.ToString("yyyy-MM-dd"),o.Product,o.Price.ToString(CultureInfo.InvariantCulture),o.Quantity.ToString(CultureInfo.InvariantCulture),o.Status};
                    writer.WriteLine(string.Join(";",row.Select(Escape)));
                }
            }
            File.Move(temporary,fullPath,true);
        }
        finally {if(File.Exists(temporary))File.Delete(temporary);}
    }
    public List<Order_KIA> Load(string path)
    {
        var rows=Parse(File.ReadAllText(path));if(rows.Count==0)throw new FormatException("CSV пуст. Ожидается строка заголовков.");
        if(!rows[0].SequenceEqual(Header))throw new FormatException("Неверные столбцы CSV. Используйте файл образца из папки Data.");
        var result=new List<Order_KIA>();var service=new OrderService_KIA();
        for(int i=1;i<rows.Count;i++)
        {
            var r=rows[i];if(r.Count==1&&r[0].Length==0)continue;
            try
            {
                if(r.Count!=12)throw new FormatException("Ожидается 12 столбцов.");
                var o=new Order_KIA {Number=r[0].Trim(),LastName=r[1],FirstName=r[2],Patronymic=r[3],Account=r[4],Address=r[5],Phone=r[6],DueDate=DateTime.ParseExact(r[7],"yyyy-MM-dd",CultureInfo.InvariantCulture),Product=r[8],Price=decimal.Parse(r[9].Replace(',','.'),NumberStyles.AllowDecimalPoint|NumberStyles.AllowLeadingSign,CultureInfo.InvariantCulture),Quantity=int.Parse(r[10],CultureInfo.InvariantCulture),Status=r[11]};
                service.Validate(o,result);result.Add(o);
            }
            catch(Exception ex) when(ex is FormatException||ex is ArgumentException||ex is OverflowException){throw new FormatException($"Запись {i+1}: {ex.Message}",ex);}
        }
        return result;
    }
    private static List<List<string>> Parse(string text)
    {
        var rows=new List<List<string>>();var row=new List<string>();var cell=new StringBuilder();bool quoted=false,closed=false;
        for(int i=0;i<text.Length;i++)
        {
            char c=text[i];
            if(quoted)
            {
                if(c=='"'){if(i+1<text.Length&&text[i+1]=='"'){cell.Append('"');i++;}else{quoted=false;closed=true;}}
                else cell.Append(c);
            }
            else if(c==';'||c=='\r'||c=='\n')
            {
                row.Add(cell.ToString());cell.Clear();closed=false;
                if(c!=';'){rows.Add(row);row=new List<string>();if(c=='\r'&&i+1<text.Length&&text[i+1]=='\n')i++;}
            }
            else if(c=='"'&&cell.Length==0&&!closed)quoted=true;
            else {if(closed||c=='"')throw new FormatException("Некорректные кавычки CSV.");cell.Append(c);}
        }
        if(quoted)throw new FormatException("Незакрытые кавычки CSV.");
        if(cell.Length>0||row.Count>0||closed){row.Add(cell.ToString());rows.Add(row);}return rows;
    }
}
