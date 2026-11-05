using Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
namespace Tyuiu.KonevIA.Sprint7.Project.V10;
internal sealed class OrderChart_KIA : Control
{
    public List<Order_KIA> Orders {get;set;}=new();
    public OrderChart_KIA(){Name="chartOrdersByStatus_KIA";DoubleBuffered=true;BackColor=Color.White;Dock=DockStyle.Fill;}
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);var g=e.Graphics;g.SmoothingMode=System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using var font_KIA=new Font("Segoe UI",10F,FontStyle.Bold);
        g.DrawString("Стоимость заказов по статусам, ₽",font_KIA,Brushes.MidnightBlue,16,12);
        if(Orders.Count==0){g.DrawString("Нет данных для диаграммы",Font,Brushes.Gray,20,55);return;}
        var totals_KIA=OrderService_KIA.Statuses.Select(s=>Orders.Where(o=>o.Status==s).Sum(o=>o.Total)).ToArray();decimal max_KIA=totals_KIA.Max();if(max_KIA==0)max_KIA=1;
        float width_KIA=Width-32,height_KIA=(Height-55)/4F;if(width_KIA<40||height_KIA<20)return;
        Color[] colors_KIA={Color.SteelBlue,Color.DarkOrange,Color.SeaGreen,Color.SlateGray};
        for(int i=0;i<4;i++)
        {
            float y_KIA=50+i*height_KIA;g.DrawString(OrderService_KIA.Statuses[i],Font,Brushes.Black,12,y_KIA);
            using var brush_KIA=new SolidBrush(colors_KIA[i]);float bar_KIA=(float)(totals_KIA[i]/max_KIA)*width_KIA;
            g.FillRectangle(brush_KIA,16,y_KIA+20,Math.Max(1,bar_KIA),Math.Max(3,Math.Min(8,height_KIA-23)));
            using var alignment_KIA=new StringFormat{Alignment=StringAlignment.Far};
            g.DrawString(totals_KIA[i].ToString("N2"),Font,Brushes.Black,new RectangleF(110,y_KIA,Width-126,20),alignment_KIA);
        }
    }
}
