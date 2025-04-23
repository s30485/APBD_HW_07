using System.ComponentModel.DataAnnotations;

namespace APBD_HW_07.Domain
{
    public abstract class Device
    {
        [Key]
        public string Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        public bool IsEnabled { get; set; }
        
        public abstract void TurnOn();
        public void TurnOff() => IsEnabled = false;
    }
}