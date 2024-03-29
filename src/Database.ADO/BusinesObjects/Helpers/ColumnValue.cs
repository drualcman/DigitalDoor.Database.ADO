﻿namespace Database.ADO.BusinesObjects.Helpers;

internal sealed class ColumnValue
{
    readonly ReadOnlyCollection<TableName> Tables;
    readonly object Item;

    public ColumnValue(IList<TableName> tables, object item)
    {
        Tables = new ReadOnlyCollection<TableName>(tables);
        Item = item;
    }

    public void SetValue(Columns column, DataRow row)
    {
        SetValue(column, row[column.ColumnName]);
    }

    public void SetValue(Columns column, object row)
    {
        try
        {
            if(column.TableIndex > 0)
                SetValue(column.PropertyType, column.Column, Item.GetPropValue(Tables[column.TableIndex].Instance.Name), row);
            else
            {
                SetValue(column.PropertyType, column.Column, Item, row);
            }
        }
        catch(Exception ex)
        {
            string err = ex.Message;
            Console.WriteLine("ex 2 {0}, column: {1}, value: {2}", err, column.ColumnName, row);
            SetValue(column.Column, Item, row);
        }

    }

    private void SetValue(PropertyInfo sender, object destination, object value)
    {
        MethodInfo set = sender.GetSetMethod();
        if(set is not null)
        {
            try
            {
                if(value != null && value.GetType() != typeof(DBNull))
                    sender.SetValue(destination, Convert.ChangeType(value, Nullable.GetUnderlyingType(sender.PropertyType)));
            }
            catch(Exception ex)
            {
                string err = ex.Message;
                Console.WriteLine("ex 3 {0}, sender: {1}, value: {2}, destination: {3}", err, sender.Name, value, destination);
                Console.WriteLine("ex 3 {0}", err);
            }
        }

    }

    public void SetValue(string propertyType, PropertyInfo sender, object destination, object value)
    {
        if(value.GetType() != typeof(DBNull))
        {
            MethodInfo set = sender.GetSetMethod();
            if(set is not null)
            {
                switch(propertyType)
                {
                    case "bool":
                        if(int.TryParse(value.ToString(), out int test))
                        {
                            sender.SetValue(destination, Convert.ToBoolean(test));
                        }
                        else
                        {
                            string checkTrue = value.ToString().ToLower();
                            switch(checkTrue)
                            {
                                case "true":
                                    sender.SetValue(destination, true);
                                    break;
                                case "false":
                                default:
                                    sender.SetValue(destination, false);
                                    break;
                            }
                        }
                        break;
                    case "int":
                        sender.SetValue(destination, Convert.ToInt32(value));
                        break;
                    case "double":
                        sender.SetValue(destination, Convert.ToDouble(value));
                        break;
                    case "float":
                        sender.SetValue(destination, Convert.ToSingle(value));
                        break;
                    case "decimal":
                        sender.SetValue(destination, Convert.ToDecimal(value));
                        break;
                    case "long":
                        sender.SetValue(destination, Convert.ToInt64(value));
                        break;
                    case "short":
                        sender.SetValue(destination, Convert.ToInt16(value));
                        break;
                    case "byte":
                        sender.SetValue(destination, Convert.ToByte(value));
                        break;
                    case "time":
                        sender.SetValue(destination, ConvertIntoTime(value));
                        break;
                    case "date":
                        sender.SetValue(destination, ConvertIntoDate(value));
                        break;
                    case "nullable":
                        sender.SetValue(destination, value);
                        break;
                    default:
                        if(sender.PropertyType.Equals(value.GetType())) sender.SetValue(destination, value);
                        else
                        {
                            if(sender.PropertyType.BaseType == typeof(Enum))
                            {
                                if(int.TryParse(value.ToString(), out int index))
                                    sender.SetValue(destination, index);
                                else
                                {
                                    sender.SetValue(destination, Enum.Parse(sender.PropertyType, value.ToString()));
                                }
                            }
                            else if(sender.PropertyType == typeof(DateTime)) sender.SetValue(destination, ConvertIntoDate(value));
                            else if(sender.PropertyType == typeof(string)) sender.SetValue(destination, value.ToString());
                            else if(value.GetType().IsAssignableTo(typeof(IConvertible))) sender.SetValue(destination, Convert.ChangeType(value, sender.PropertyType));
                            else sender.SetValue(destination, value);
                        }
                        break;
                }
            }
        }
    }

    private TimeSpan ConvertIntoTime(object value)
    {
        if(!TimeSpan.TryParse(value.ToString(), out TimeSpan time))
            time = new TimeSpan(DateTime.Now.Ticks);
        return time;
    }
    private DateTime ConvertIntoDate(object value)
    {
        Type type = value.GetType();
        DateTime date;
        if(type == typeof(TimeSpan))
        {
            TimeSpan time = ConvertIntoTime(value);
            date = DateTime.Today.AddTicks(time.Ticks);
        }
        else 
        {
            try
            {
                date = Convert.ToDateTime(value);
            }
            catch(Exception ex)
            {
                string err = ex.Message;
                Console.WriteLine("ex ConvertIntoDate {0}", err);

                if(!DateTime.TryParse(value.ToString(), out date))
                {
                    if(!TimeSpan.TryParse(value.ToString(), out TimeSpan time))
                        time = new TimeSpan(DateTime.Now.Ticks);
                    date = new DateTime(time.Ticks);
                }
            }
        }
        return date;
    }
}
