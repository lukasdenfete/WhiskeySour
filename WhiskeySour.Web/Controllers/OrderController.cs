using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhiskeySour.Infrastructure;


namespace WhiskeySour.Controllers;

public class OrderController : Controller
{
    private readonly AppDbContext _context;

    public OrderController(AppDbContext context)
    {
        _context = context;
    }
    
}