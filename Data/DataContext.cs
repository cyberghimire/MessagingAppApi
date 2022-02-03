using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MessagingApp.API.Models;



namespace MessagingApp.API.Data
{
    public class DataContext : DbContext
    {
        //DbContext must have an instance of DbContextOptions in order for it to work. We do this by passing these options up into the base constructor of the DbContext.
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        //In our DataContext class, in order to tell EntityFramework about our entities, we need to give it some properties. These properties are going to be of type DbSet<Value>. Here, Value represents our Entity. We also need to use our Models namespace. 
        //Here, Values is going to be the table name in SQL once our database is created. After doing this, we need to tell our Application about this. So, in our Startup.cs class, we add this as a service.  
        
        //Migrations:
        //Migrations provide a way to incrementally apply changes to the database to keep in sync with our EF core model whilst preserving a data in the database. So, when we add a migration, EF is going to take a look for AddDbContext service that we add in the 
        //Startup.cs file. This file contains Reference to the Data/DataContext.cs class and it's going to create a table in our database based on the DbSet that we specified here. 
        

        public DbSet<User> Users {get; set;}

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder){
            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m=> m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m=> m.MessagesRecieved)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}