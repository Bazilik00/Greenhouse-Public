using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Entities;

[Index(nameof(DateTime))]
public class ChartsData
{
    [Key]
    public long Id { get; set; }
    
    public DateTime DateTime { get; set; }

    public double Value { get; set; }

    public required string Source { get; set; }
}