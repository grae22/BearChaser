using AutoMapper;
using BearChaser.DataTransferObjects;
using BearChaser.Models;

namespace BearChaser
{
  internal static class MapperConfig
  {
    //---------------------------------------------------------------------------------------------

    private static bool _isInitialised;

    //---------------------------------------------------------------------------------------------

    public static void Initialise()
    {
      if (_isInitialised)
      {
        return;
      }

      Mapper.Initialize(cfg => cfg.CreateMap<Goal, GoalData>());

      _isInitialised = true;
    }

    //---------------------------------------------------------------------------------------------
  }
}