using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Unity;

namespace BearChaser.Factories
{
  public class ControllerFactory : IControllerFactory
  {
    //---------------------------------------------------------------------------------------------

    private readonly IUnityContainer _container = new UnityContainer();
    private readonly IControllerFactory _defaultFactory = new DefaultControllerFactory();

    //---------------------------------------------------------------------------------------------

    public IController CreateController(RequestContext requestContext, string controllerName)
    {
      try
      {
        return _container.Resolve<IController>(controllerName);
      }
      catch (Exception)
      {
        return _defaultFactory.CreateController(requestContext, controllerName);
      }
    }

    //---------------------------------------------------------------------------------------------

    public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
    {
      return SessionStateBehavior.Default;
    }

    //---------------------------------------------------------------------------------------------

    public void ReleaseController(IController controller)
    {
    }

    //---------------------------------------------------------------------------------------------

    public void RegisterController<T>(string name) where T : IController
    {
      _container.RegisterType<IController, T>(name);
    }

    //---------------------------------------------------------------------------------------------

    public void RegisterType<TInterface, TConcrete>() where TConcrete : TInterface
    {
      _container.RegisterType<TInterface, TConcrete>();
    }

    //---------------------------------------------------------------------------------------------

    public void RegisterInstance<TInterface, TConcrete>(TConcrete instance) where TConcrete : TInterface
    {
      _container.RegisterInstance<TInterface>(instance);
    }

    //---------------------------------------------------------------------------------------------
  }
}