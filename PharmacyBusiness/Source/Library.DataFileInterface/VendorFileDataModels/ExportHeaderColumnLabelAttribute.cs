namespace Library.DataFileInterface.VendorFileDataModels
{
    public class ExportHeaderColumnLabelAttribute : Attribute
    {
        public string Name { get; private set; }

        public ExportHeaderColumnLabelAttribute(string name)
        {
            this.Name = name;
        }
    }
}
