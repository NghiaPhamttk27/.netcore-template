namespace netcoretemplate.Models
{
    public class Chucvu
    {
        public int Id { get; set; }          // Khóa chính
        public string Name { get; set; }     // Tên chức vụ
        public bool Active { get; set; }     // Trạng thái (true = hoạt động, false = không hoạt động)
    }
}
