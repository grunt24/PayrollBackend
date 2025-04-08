using payroll_system.Core.Entities;

namespace PayrollSystemBackend.ServiceRepository.InterfaceRepository
{
    public interface IPositionService
    {
        bool AddPosition(Position position); // Create a new position
        void DeletePosition(int id); // Delete a position by ID
        void UpdatePosition(Position position, int id); // Update an existing position by ID
        IEnumerable<Position> GetAllPositions(); // Get all positions, optionally including departments
        Position GetPositionById(int id); // Get a single position by ID, optionally including department
    }
}
