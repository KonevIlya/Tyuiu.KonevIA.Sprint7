namespace Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
public sealed class Order_KIA
{
    public string Number {get;set;}="";
    public string LastName {get;set;}="";
    public string FirstName {get;set;}="";
    public string Patronymic {get;set;}="";
    public string Account {get;set;}="";
    public string Address {get;set;}="";
    public string Phone {get;set;}="";
    public DateTime DueDate {get;set;}=DateTime.Today;
    public string Product {get;set;}="";
    public decimal Price {get;set;}
    public int Quantity {get;set;}=1;
    public string Status {get;set;}="Новый";
    public decimal Total => Price*Quantity;
    public string Customer => $"{LastName} {FirstName} {Patronymic}".Trim();
    public Order_KIA Copy() => (Order_KIA)MemberwiseClone();
}
public sealed record Statistics_KIA(int Count,long Quantity,decimal Sum,decimal Average,decimal Min,decimal Max);
public sealed class OrderService_KIA
{
    public static readonly string[] Statuses={"Новый","В работе","Выполнен","Отменён"};
    public void Validate(Order_KIA order,IEnumerable<Order_KIA> existing,string originalNumber=null)
    {
        if(string.IsNullOrWhiteSpace(order.Number)||string.IsNullOrWhiteSpace(order.LastName)||string.IsNullOrWhiteSpace(order.FirstName)||string.IsNullOrWhiteSpace(order.Product))
            throw new ArgumentException("Заполните номер заказа, фамилию, имя и название товара.");
        if(existing.Any(x=>x.Number!=originalNumber&&string.Equals(x.Number?.Trim(),order.Number.Trim(),StringComparison.OrdinalIgnoreCase)))throw new ArgumentException("Заказ с таким номером уже существует.");
        if(new[]{order.Number,order.LastName,order.FirstName,order.Patronymic,order.Account,order.Address,order.Phone,order.Product}.Any(x=>x==null||x.Length>500))throw new ArgumentException("Текстовые поля не должны превышать 500 символов.");
        if(order.DueDate.Date<new DateTime(1753,1,1)||order.DueDate.Date>new DateTime(9998,12,31))throw new ArgumentException("Дата исполнения должна быть в диапазоне 01.01.1753–31.12.9998.");
        if(order.Price>1000000000000m||order.Quantity>1000000)throw new ArgumentException("Максимальная цена — 1 трлн ₽, количество — 1 000 000.");
        if(order.Price<0||order.Quantity<=0)throw new ArgumentException("Цена не может быть отрицательной, количество должно быть больше нуля.");
        if(!Statuses.Contains(order.Status))throw new ArgumentException("Неизвестный статус заказа.");
        try { _=order.Total; } catch(OverflowException) {throw new ArgumentException("Стоимость заказа слишком велика.");}
    }
    public List<Order_KIA> Query(IEnumerable<Order_KIA> source,string search="",string status=null,DateTime? from=null,DateTime? to=null,string sort="Дата ↑")
    {
        if(from.HasValue&&to.HasValue&&from.Value.Date>to.Value.Date)return new List<Order_KIA>();
        var query=source.Where(x=>(string.IsNullOrWhiteSpace(search)||string.Join(" ",x.Number,x.Customer,x.Account,x.Address,x.Phone,x.Product).Contains(search.Trim(),StringComparison.OrdinalIgnoreCase))
            &&(string.IsNullOrEmpty(status)||x.Status==status)&&(!from.HasValue||x.DueDate.Date>=from.Value.Date)&&(!to.HasValue||x.DueDate.Date<=to.Value.Date));
        return (sort switch {
            "Дата ↓"=>query.OrderByDescending(x=>x.DueDate),"Сумма ↑"=>query.OrderBy(x=>x.Total),"Сумма ↓"=>query.OrderByDescending(x=>x.Total),
            "Клиент А–Я"=>query.OrderBy(x=>x.Customer,StringComparer.CurrentCultureIgnoreCase),"Номер"=>query.OrderBy(x=>x.Number,StringComparer.OrdinalIgnoreCase),_=>query.OrderBy(x=>x.DueDate)
        }).ToList();
    }
    public bool CanExportTo(string exportPath,string currentPath) => string.IsNullOrEmpty(currentPath)||!string.Equals(Path.GetFullPath(exportPath),Path.GetFullPath(currentPath),StringComparison.OrdinalIgnoreCase);
    public Statistics_KIA Statistics(IEnumerable<Order_KIA> source)
    {
        var items=source.ToList();return items.Count==0?new(0,0,0,0,0,0):new(items.Count,items.Sum(x=>(long)x.Quantity),items.Sum(x=>x.Total),items.Average(x=>x.Total),items.Min(x=>x.Total),items.Max(x=>x.Total));
    }
}


