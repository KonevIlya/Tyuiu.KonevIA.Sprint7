using Tyuiu.KonevIA.Sprint7.Project.V10.Lib;
namespace Tyuiu.KonevIA.Sprint7.Project.V10;
internal sealed class FormEditOrder_KIA : Form
{
    public Order_KIA Order {get;private set;}
    private readonly Dictionary<string,TextBox> fields_KIA=new();
    private readonly DateTimePicker dateDue_KIA=new(){Name="dateTimePickerDueDate_KIA",Format=DateTimePickerFormat.Short,Dock=DockStyle.Fill};
    private readonly NumericUpDown price_KIA=new(){Name="numericUpDownPrice_KIA",Maximum=1000000000000m,DecimalPlaces=2,Dock=DockStyle.Fill};
    private readonly NumericUpDown quantity_KIA=new(){Name="numericUpDownQuantity_KIA",Minimum=1,Maximum=1000000,Dock=DockStyle.Fill};
    private readonly ComboBox status_KIA=new(){Name="comboBoxStatus_KIA",DropDownStyle=ComboBoxStyle.DropDownList,Dock=DockStyle.Fill};
    private readonly ErrorProvider errorProviderFields_KIA=new(){BlinkStyle=ErrorBlinkStyle.NeverBlink};
    private readonly Label labelOrderTotal_KIA=new(){Name="labelOrderTotal_KIA",AutoSize=true,ForeColor=Color.FromArgb(28,76,112),Font=new Font("Segoe UI",11F,FontStyle.Bold)};
    private readonly IEnumerable<Order_KIA> existing_KIA;
    private readonly string original_KIA;
    public FormEditOrder_KIA(Order_KIA order,IEnumerable<Order_KIA> existing,bool edit)
    {
        Name="formEditOrder_KIA";Text=edit?"Редактирование заказа":"Новый заказ";ClientSize=new Size(570,620);MinimumSize=new Size(550,650);StartPosition=FormStartPosition.CenterParent;Font=new Font("Segoe UI",10F);
        BackColor=Color.FromArgb(247,249,252);errorProviderFields_KIA.ContainerControl=this;
        Order=order.Copy();existing_KIA=existing;original_KIA=edit?order.Number:null;
        var layout_KIA=new TableLayoutPanel{Name="tableLayoutOrderFields_KIA",Dock=DockStyle.Fill,ColumnCount=2,RowCount=15,Padding=new Padding(20),AutoScroll=true};
        layout_KIA.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,175));layout_KIA.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
        string[] keys={"Number","LastName","FirstName","Patronymic","Account","Address","Phone","Product"};
        string[] labels={"Номер заказа *","Фамилия *","Имя *","Отчество","Номер счёта","Адрес","Телефон","Товар *"};
        for(int i=0;i<keys.Length;i++)
        {
            var text_KIA=new TextBox{Name="textBox"+keys[i]+"_KIA",Dock=DockStyle.Fill,MaxLength=500,Text=(string)typeof(Order_KIA).GetProperty(keys[i]).GetValue(order)};
            string key_KIA=keys[i];
            text_KIA.TextChanged+=(s,e)=>ValidateField(key_KIA);
            fields_KIA.Add(keys[i],text_KIA);AddRow(layout_KIA,i,labels[i],text_KIA,keys[i]);
        }
        dateDue_KIA.Value=order.DueDate;price_KIA.Value=order.Price;quantity_KIA.Value=order.Quantity;
        status_KIA.Items.AddRange(OrderService_KIA.Statuses);status_KIA.SelectedItem=order.Status;
        AddRow(layout_KIA,8,"Дата исполнения",dateDue_KIA,"DueDate");AddRow(layout_KIA,9,"Цена, ₽",price_KIA,"Price");AddRow(layout_KIA,10,"Количество",quantity_KIA,"Quantity");AddRow(layout_KIA,11,"Статус",status_KIA,"Status");
        var note_KIA=new Label{Name="labelRequiredFields_KIA",Text="* Обязательные поля подсвечены до заполнения.",AutoSize=true,ForeColor=Color.DimGray};layout_KIA.Controls.Add(note_KIA,0,12);layout_KIA.SetColumnSpan(note_KIA,2);
        layout_KIA.Controls.Add(labelOrderTotal_KIA,0,13);layout_KIA.SetColumnSpan(labelOrderTotal_KIA,2);
        price_KIA.ValueChanged+=(s,e)=>UpdateTotal();quantity_KIA.ValueChanged+=(s,e)=>UpdateTotal();UpdateTotal();
        var buttons_KIA=new FlowLayoutPanel{Name="flowLayoutEditorActions_KIA",Dock=DockStyle.Fill,FlowDirection=FlowDirection.RightToLeft};
        var cancel_KIA=new Button{Name="buttonCancelOrder_KIA",Text="Отмена",DialogResult=DialogResult.Cancel,AutoSize=true};
        var save_KIA=new Button{Name="buttonSaveOrder_KIA",Text="Сохранить",AutoSize=true};save_KIA.Click+=SaveOrder;
        buttons_KIA.Controls.AddRange(new Control[]{cancel_KIA,save_KIA});layout_KIA.Controls.Add(buttons_KIA,0,14);layout_KIA.SetColumnSpan(buttons_KIA,2);
        for(int i=0;i<12;i++)layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,38));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,40));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,34));layout_KIA.RowStyles.Add(new RowStyle(SizeType.Absolute,48));
        StyleButton(save_KIA,true);StyleButton(cancel_KIA,false);
        foreach(var key_KIA in keys)ValidateField(key_KIA);
        Controls.Add(layout_KIA);AcceptButton=save_KIA;CancelButton=cancel_KIA;
        var tips_KIA=new ToolTip();foreach(var field in fields_KIA)tips_KIA.SetToolTip(field.Value,"Введите "+labels[Array.IndexOf(keys,field.Key)].TrimEnd(' ','*'));tips_KIA.SetToolTip(price_KIA,"Цена единицы товара в рублях");tips_KIA.SetToolTip(quantity_KIA,"Количество товара: положительное целое число");tips_KIA.SetToolTip(status_KIA,"Текущий статус заказа");tips_KIA.SetToolTip(dateDue_KIA,"Дата исполнения заказа");FormClosed+=(s,e)=>{tips_KIA.Dispose();errorProviderFields_KIA.Dispose();};
    }
    private void UpdateTotal()=>labelOrderTotal_KIA.Text=$"Стоимость заказа: {price_KIA.Value*quantity_KIA.Value:N2} ₽";
    private bool ValidateField(string key)
    {
        var field_KIA=fields_KIA[key];
        bool required_KIA=key is "Number" or "LastName" or "FirstName" or "Product";
        string error_KIA=required_KIA&&string.IsNullOrWhiteSpace(field_KIA.Text)?"Обязательное поле":
            key=="Number"&&existing_KIA.Any(x=>x.Number!=original_KIA&&string.Equals(x.Number,field_KIA.Text.Trim(),StringComparison.OrdinalIgnoreCase))?"Этот номер заказа уже существует":"";
        field_KIA.BackColor=error_KIA.Length>0?Color.FromArgb(255,241,235):Color.White;
        errorProviderFields_KIA.SetError(field_KIA,error_KIA);return error_KIA.Length==0;
    }
    private static void StyleButton(Button button,bool primary)
    {
        button.FlatStyle=FlatStyle.Flat;button.FlatAppearance.BorderColor=Color.FromArgb(158,177,196);
        button.BackColor=primary?Color.FromArgb(35,105,170):Color.White;button.ForeColor=primary?Color.White:Color.FromArgb(35,55,75);
        button.Padding=new Padding(12,4,12,4);button.Cursor=Cursors.Hand;button.Margin=new Padding(5);
    }
    private static void AddRow(TableLayoutPanel layout,int row,string label,Control input,string key)
    {layout.Controls.Add(new Label{Name="label"+key+"_KIA",Text=label,AutoSize=true,Anchor=AnchorStyles.Left},0,row);layout.Controls.Add(input,1,row);}
    private void SaveOrder(object sender,EventArgs e)
    {
        bool valid_KIA=true;Control firstInvalid_KIA=null;
        foreach(var key_KIA in fields_KIA.Keys)if(!ValidateField(key_KIA)){valid_KIA=false;firstInvalid_KIA??=fields_KIA[key_KIA];}
        if(!valid_KIA){firstInvalid_KIA.Focus();return;}
        foreach(var pair in fields_KIA)typeof(Order_KIA).GetProperty(pair.Key).SetValue(Order,pair.Value.Text.Trim());
        Order.DueDate=dateDue_KIA.Value.Date;Order.Price=price_KIA.Value;Order.Quantity=(int)quantity_KIA.Value;Order.Status=status_KIA.Text;
        try {new OrderService_KIA().Validate(Order,existing_KIA,original_KIA);DialogResult=DialogResult.OK;}
        catch(ArgumentException ex){MessageBox.Show(this,ex.Message,"Проверьте данные",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
    }
}



