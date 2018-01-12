using AutoMapper;
using BearChaser.DataTransferObjects;
using BearChaser.Models;

namespace BearChaser
{
  internal static class MapperConfig
  {
    //---------------------------------------------------------------------------------------------

    public static void Initialise()
    {
      Mapper.Initialize(cfg => cfg.CreateMap<Goal, GoalData>());
    }

    //---------------------------------------------------------------------------------------------
  }
}