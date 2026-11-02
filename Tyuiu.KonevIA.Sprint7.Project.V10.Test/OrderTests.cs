using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
namespace Tyuiu.KonevIA.Sprint7.Project.V10.Test;
[TestClass]
public class OrderTests_KIA
{
    private static Order_KIA Order(string number,decimal price=10,int quantity=2)=>new(){Number=number,LastName="Иванов",FirstName="Иван",Product="Товар",Price=price,Quantity=quantity,DueDate=new DateTime(2026,11,10)};
    [TestMethod]
    public void CsvRoundTripPreservesQuotedMultilineFields()
    {
        string path_KIA=Path.GetTempFileName();
        try
        {
            var order_KIA=Order("001",12.50m,3);order_KIA.Address="Дом; корпус \"А\"\r\nкв. 7";order_KIA.Phone="+7 000";
            var repository_KIA=new CsvRepository_KIA();repository_KIA.Save(path_KIA,new[]{order_KIA});var loaded_KIA=repository_KIA.Load(path_KIA).Single();
            Assert.AreEqual(order_KIA.Address,loaded_KIA.Address);Assert.AreEqual("001",loaded_KIA.Number);Assert.AreEqual(37.5m,loaded_KIA.Total);Assert.AreEqual(order_KIA.DueDate,loaded_KIA.DueDate);
        }
        finally{File.Delete(path_KIA);}
    }
    [TestMethod]
    public void QueryCombinesSearchStatusDatesAndSort()
    {
        var a_KIA=Order("A",30);var b_KIA=Order("B",10);var c_KIA=Order("C",50);c_KIA.Status="Выполнен";
        var result_KIA=new OrderService_KIA().Query(new[]{a_KIA,b_KIA,c_KIA},"ИВАН","Новый",new DateTime(2026,11,10),new DateTime(2026,11,10),"Сумма ↑");
        CollectionAssert.AreEqual(new[]{"B","A"},result_KIA.Select(o=>o.Number).ToArray());
    }
    [TestMethod]
    public void StatisticsUseOrderTotalsAndHandleEmptySelection()
    {
        var service_KIA=new OrderService_KIA();var stats_KIA=service_KIA.Statistics(new[]{Order("A",10,2),Order("B",30,1)});
        Assert.AreEqual(50m,stats_KIA.Sum);Assert.AreEqual(25m,stats_KIA.Average);Assert.AreEqual(20m,stats_KIA.Min);Assert.AreEqual(30m,stats_KIA.Max);Assert.AreEqual(3,stats_KIA.Quantity);
        Assert.AreEqual(0m,service_KIA.Statistics(Array.Empty<Order_KIA>()).Average);
    }
    [TestMethod]
    public void ValidationRejectsDuplicateAndInvalidQuantity()
    {
        var service_KIA=new OrderService_KIA();var original_KIA=Order("A");
        Assert.ThrowsException<ArgumentException>(()=>service_KIA.Validate(Order("a"),new[]{original_KIA}));
        service_KIA.Validate(original_KIA,new[]{original_KIA},"A");
        Assert.ThrowsException<ArgumentException>(()=>service_KIA.Validate(Order("B",1,0),Array.Empty<Order_KIA>()));
    }
    [TestMethod]
    public void CsvRejectsBrokenRecordsWithoutPartialLoad()
    {
        string path_KIA=Path.GetTempFileName();
        try{File.WriteAllText(path_KIA,"\"Незакрытая строка");Assert.ThrowsException<FormatException>(()=>new CsvRepository_KIA().Load(path_KIA));}
        finally{File.Delete(path_KIA);}
    }
    [TestMethod]
    public void InvertedDateRangeReturnsEmptySelection()
    {
        Assert.AreEqual(0,new OrderService_KIA().Query(new[]{Order("A")},from:new DateTime(2026,11,12),to:new DateTime(2026,11,1)).Count);
    }
    [TestMethod]
    public void LargeQuantityStatisticsDoNotOverflow()
    {
        var orders_KIA=Enumerable.Range(0,3000).Select(i=>Order(i.ToString(),1,1000000));
        Assert.AreEqual(3000000000L,new OrderService_KIA().Statistics(orders_KIA).Quantity);
    }
    [TestMethod]
    public void ExportCannotOverwriteCurrentDatabase()
    {
        var service_KIA=new OrderService_KIA();string path_KIA=Path.Combine(Path.GetTempPath(),"Orders.csv");
        Assert.IsFalse(service_KIA.CanExportTo(path_KIA,path_KIA.ToUpperInvariant()));
        Assert.IsTrue(service_KIA.CanExportTo(Path.Combine(Path.GetTempPath(),"selection.csv"),path_KIA));
    }
    [TestMethod]
    public void CopyForEditingDoesNotModifyOriginal()
    {
        var original_KIA=Order("A");var draft_KIA=original_KIA.Copy();draft_KIA.Product="Изменённый товар";
        Assert.AreEqual("Товар",original_KIA.Product);
    }
    [TestMethod]
    public void ValidationRejectsUnsupportedDateAndOversizedFields()
    {
        var service_KIA=new OrderService_KIA();var order_KIA=Order("A");order_KIA.DueDate=new DateTime(1000,1,1);
        Assert.ThrowsException<ArgumentException>(()=>service_KIA.Validate(order_KIA,Array.Empty<Order_KIA>()));
        order_KIA.DueDate=DateTime.Today;order_KIA.Address=new string('x',501);
        Assert.ThrowsException<ArgumentException>(()=>service_KIA.Validate(order_KIA,Array.Empty<Order_KIA>()));
    }
}
