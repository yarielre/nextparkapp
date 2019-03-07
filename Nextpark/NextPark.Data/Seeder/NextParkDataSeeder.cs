using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using NextPark.Domain.Entities;
using NextPark.Enums.Enums;

namespace NextPark.Data.Seeder
{
    public static class NextParkDataSeeder
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                CreateUserAsync(serviceProvider).Wait();
                context.Parkings.AddRange(CreateParkings());
                context.SaveChanges();
                context.Events.AddRange(CreateEvent());
                context.Feeds.AddRange(CreateFeeds());
                context.SaveChanges();
            }
        }

        private static async Task CreateUserAsync(IServiceProvider serviceProvider)
        {
            using (var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>())
            {
                var user = new ApplicationUser
                {
                    Name = "demouser",
                    Lastname = "Nextpark",
                    Email = "info@nextpark.ch",
                    UserName = "info@nextpark.ch",
                    PhoneNumber = "11111",
                    Balance = 1000
                };
                try
                {
                    var result = await userManager.CreateAsync(user, "NextPark.1").ConfigureAwait(false);
                    if (!result.Succeeded)
                        throw new Exception("Data seeder error trying to create admin user");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw new Exception("Data seeder error trying to create admin user");
                }
            }
        }

        private static List<Parking> CreateParkings()
        {
            return new List<Parking>
            {
                new Parking
                {
                    Address = "Via Strada",
                    Cap = 7777,
                    City = "Lugano",
                    CarPlate = "TI 000000",
                    Latitude = 40,
                    Longitude = 40,
                    PriceMax = 4,
                    PriceMin = 4,
                    State = "Ticino",
                    Status = ParkingStatus.Enabled,
                    UserId = 1,
                    ImageUrl = "image_parking1.png",
                    //Events = new List<Event>
                    //{
                    //    new Event
                    //    {
                    //        StartDate = DateTime.Now,
                    //        EndDate = DateTime.Now.AddYears(1),
                    //        RepetitionEndDate = DateTime.Now.AddYears(1),
                    //        RepetitionType = RepetitionType.Dayly,
                    //        RepetitionId = new Guid()
                    //    }
                    //}
                }
            };
        }
        private static List<Feed> CreateFeeds()
        {
            return new List<Feed>
            {
                new Feed
                {
                  Name = "RentEarningPercent",
                  Tax = 19
                }
            };
        }

        private static List<Event> CreateEvent()
        {
            return new List<Event>
            {
                new Event
                    {
                        StartDate = DateTime.Now,
                        ParkingId = 1,
                        EndDate = DateTime.Now.AddYears(1),
                        RepetitionEndDate = DateTime.Now.AddYears(1),
                        RepetitionType = RepetitionType.Dayly,
                        RepetitionId = new Guid()
                    }
            };
        }

    }
}