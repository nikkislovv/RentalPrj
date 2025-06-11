using RentCommandApi.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates
{
    public class RentAggregate : AggregateRoot
    {
        private DateTime _start;
        private DateTime _finish;
        private decimal _totalPrice;
        private string _userId;
        private Guid _carId;
        private DateTime _updatedAt;
        private string _status;

        public DateTime Start { get => _start; }
        public DateTime Finish { get => _finish; }
        public string Status { get => _status; }
        public Guid CarId { get => _carId; }

        public RentAggregate()
        {
        }

        public RentAggregate(
            Guid id,
            string userId,
            DateTime start,
            DateTime finish,
            Guid carId,
            decimal dayCost)
        {
            RaiseEvent(new RentCreatedEvent
            {
                Id = id,
                UserId=userId,
                Start = start,
                Finish = finish,
                CarId = carId,
                TotalPrice = PriceCalculation(start, finish, dayCost),
                Status = "Awaiting",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public void Apply(
            RentCreatedEvent @event)
        {
            _id = @event.Id;
            _userId = @event.UserId;
            _start = @event.Start;
            _finish = @event.Finish;
            _totalPrice = @event.TotalPrice;
            _carId = @event.CarId;
            _updatedAt = @event.UpdatedAt;
            _status = @event.Status;
        }

        public void StartRent()
        {
            if (_status == "Driving" || _status == "Finished")
                throw new InvalidOperationException("You cannot start the rent of driving or finished status!");

            if (_start >= DateTime.Now)
                throw new InvalidOperationException("You cannot start the rent now!");

            RaiseEvent(new RentStartedEvent
            {
                Id = _id,
                Status = "Driving",
                UpdatedAt = DateTime.UtcNow
            });
        }

        public void Apply(
            RentStartedEvent @event)
        {
            _id = @event.Id;
            _updatedAt = @event.UpdatedAt;
            _status = @event.Status;
        }

        public void ExtendRent(
            DateTime finish,
            decimal dayCost)
        {
            if (_status == "Awaiting" || _status == "Finished")
                throw new InvalidOperationException("You cannot extend the rent of awaiting or finished status!");

            if (finish <= _finish)
                throw new InvalidOperationException("The finish date is entered incorrectly. Please provide a valid date value!");

            RaiseEvent(new RentExtendedEvent
            {
                Id = _id,
                Finish = finish,
                TotalPrice = _totalPrice + PriceCalculation(_finish, finish, dayCost),
                Status = "Driving",
                UpdatedAt = DateTime.UtcNow
            });
        }

        public void Apply(
            RentExtendedEvent @event)
        {
            _id = @event.Id;
            _finish = @event.Finish;
            _totalPrice = @event.TotalPrice;
            _status = @event.Status;
            _updatedAt = @event.UpdatedAt;
        }

        public void FinishRent()
        {
            if (_status == "Awaiting" || _status == "Finished")
                throw new InvalidOperationException("You cannot finish the rent of awaiting or finished status!");

            RaiseEvent(new RentFinishedEvent
            {
                Id = _id,
                Status = "Finished",
                UpdatedAt = DateTime.UtcNow
            });
        }

        public void Apply(
            RentFinishedEvent @event)
        {
            _id = @event.Id;
            _updatedAt = @event.UpdatedAt;
            _status = @event.Status;
        }

        public void DeleteRent()
        {
            RaiseEvent(new RentDeletedEvent
            {
                Id = _id
            });
        }

        public void Apply(
            RentDeletedEvent @event)
        {
            _id = @event.Id;
            _status = "Finished";
        }

        private decimal PriceCalculation(
            DateTime start,
            DateTime finish,
            decimal dayCost)
        {
            if (start <= DateTime.UtcNow || finish <= DateTime.UtcNow)
                throw new InvalidOperationException("The date values are entered incorrectly. Please provide a valid date values!");
            
            var totalDays = (decimal)(finish - start).TotalHours / 24;
            decimal result;

            if (totalDays == (int)totalDays)
                result = totalDays * dayCost;
            else
                result = ((int)totalDays + 1) * dayCost;

            if (result <= 0 || (finish - start).Days <= 0)
                throw new InvalidOperationException("The date values are entered incorrectly. Please provide a valid date values!");

            return result;
        }

    }
}
