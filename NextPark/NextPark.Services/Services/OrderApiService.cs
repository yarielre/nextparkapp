﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NextPark.Data.Infrastructure;
using NextPark.Data.Repositories;
using NextPark.Domain.Entities;
using NextPark.Enums;
using NextPark.Enums.Enums;
using NextPark.Models;

namespace NextPark.Services.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Parking> _parkingRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IRepository<Feed> _feedRepository;
        private readonly IRepository<Transaction> _transactionRepository;

        public OrderApiService(
            IUnitOfWork unitOfWork,
            IRepository<Parking> parkingRepository,
            IRepository<Order> orderRepository,
            IRepository<ApplicationUser> userRepository,
            IRepository<Feed> feedRepository,
            IRepository<Transaction> transactionRepository)
        {
            _unitOfWork = unitOfWork;
            _parkingRepository = parkingRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _feedRepository = feedRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse> TerminateOrder(int orderId)
        {

            try
            {
                //Logic terminate
                //Change order state  completed
                //Scale user balance
                //Change parking state to available

                var order = _orderRepository.Find(orderId);

                if (order == null)
                    return ApiResponse.GetErrorResponse("Order not found.", ErrorType.EntityNotFound);

                if (order.OrderStatus == OrderStatus.Finished)
                {
                    return ApiResponse.GetErrorResponse("Order must be active to allow terminate the order", ErrorType.OrderCanNotBeFinished);
                }

                var parking = _parkingRepository.Find(order.ParkingId);
                if (parking == null)
                {
                    return ApiResponse.GetErrorResponse("Parking not found.", ErrorType.EntityNull);
                }
                //Parking's owned user
                var parkingOwnedUser = _userRepository.Find(parking.UserId);
                if (parkingOwnedUser == null)
                {
                    return ApiResponse.GetErrorResponse("Parking's owned user not found.", ErrorType.EntityNotFound);
                }
                var tax = await _feedRepository.FirstOrDefaultWhereAsync(f => f.Name == "RentEarningPercent").ConfigureAwait(false);
                if (tax == null)
                {
                    return ApiResponse.GetErrorResponse("Rent earning tax not found.", ErrorType.EntityNotFound);
                }
                //User who created the order
                var userOrder = _userRepository.Find(order.UserId);
                if (userOrder == null)
                {
                    return ApiResponse.GetErrorResponse("User who rented the order not found", ErrorType.EntityNotFound);
                }

                var rentEraningTax = CalCulateTax(order.Price, tax.Tax);
                order.OrderStatus = OrderStatus.Finished;
                parking.Status = ParkingStatus.Enabled;

                var renTransaction = new Transaction
                {
                    CashMoved = rentEraningTax,
                    UserId = parking.UserId,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = Guid.NewGuid(),
                    Type = TransactionType.RentTransaction
                };

                var feedTransaction = new Transaction
                {
                    CashMoved = rentEraningTax,
                    UserId = parking.UserId,
                    CreationDate = DateTime.Now,
                    CompletationDate = DateTime.Now,
                    Status = TransactionStatus.Completed,
                    TransactionId = Guid.NewGuid(),
                    Type = TransactionType.FeedTransaction
                };

                //Saving rent and feed transactions
                _transactionRepository.Add(renTransaction);
                _transactionRepository.Add(feedTransaction);

                //Update user balance when rent is finished
                userOrder.Balance = userOrder.Balance - order.Price;
                _userRepository.Update(userOrder);

                //Update user profit when rent is finished
                parkingOwnedUser.Profit = parkingOwnedUser.Profit + rentEraningTax;
                _userRepository.Update(parkingOwnedUser);

                //Update parking after status change
                _parkingRepository.Update(parking);

                //Update order after status change
                _orderRepository.Update(order);


                await _unitOfWork.CommitAsync().ConfigureAwait(false);
                return ApiResponse.GetSuccessResponse(order, "Ok");
            }
            catch (Exception e)
            {
                return ApiResponse.GetErrorResponse($"Server error: {e.Message}", ErrorType.Exception);
            }
        }

        private static double CalCulateTax(double price, double taxPorcent)
        {
            return (price * taxPorcent) / 100;
        }
    }
    
}