using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kartel.Domain.Infrastructure.Routing
{
    /// <summary>
    /// ����������� �������� ������
    /// </summary>
    public static class RoutesManager
    {
        /// <summary>
        /// ������������ ��������� ���� � ���������� ����������� ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="route">��������� ���� �����</param>
        /// <param name="routeMapping">�������</param>
        /// <param name="insertFirst">�������� �� ���� ����� ������ � ������� ������</param>
        public static void RegisterRoute(string name, string route, object routeMapping, bool insertFirst = false)
        {
            var newRoute = RouteTable.Routes.MapRoute(name, route, routeMapping);
            if (insertFirst)
            {
                RouteTable.Routes.Remove(newRoute);
                RouteTable.Routes.Insert(0,newRoute);
            }
        }

        /// <summary>
        /// ������� ��������� ���� �� ������� ������
        /// </summary>
        /// <param name="route">����</param>
        public static void RemoveRoute(string route)
        {
            var findedRoute = RouteTable.Routes.Cast<Route>().FirstOrDefault(r => r.Url.ToLower() == route.ToLower());
            if (findedRoute != null)
            {
                RouteTable.Routes.Remove(findedRoute);
            }
        }

        /// <summary>
        /// ��������� ��������� ���� �� ����� ��������
        /// </summary>
        /// <param name="oldRoute">������ ����</param>
        /// <param name="newRoute">����� ����</param>
        public static void UpdateRoute(string oldRoute, string newRoute)
        {
            var findedRoute = RouteTable.Routes.Cast<Route>().FirstOrDefault(r => r.Url.ToLower() == oldRoute.ToLower());
            if (findedRoute != null)
            {
                findedRoute.Url = newRoute;
            }
        }

        /// <summary>
        /// ��������������� �����, ����������� ��� ��������� ������ �� ������� ������� � ������� ������� ��������� RouteAttribute � ��������������� ��������� ���� �� ��� ��������������� ����� ������
        /// </summary>
        public static void RegisterActionRoutes()
        {
            // �������� ���������� � ������������ � ���������, ���������� ��������� �������� ��������� ��� ��������� ������
            var methodsMappings = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  from type in assembly.GetTypes()
                                  from method in type.GetMethods()
                                  let attributes = method.GetCustomAttributes(typeof (RouteAttribute), false)
                                  where attributes.Length > 0
                                  select
                                      new
                                          {
                                              Controller = type.Name,
                                              Action = method.Name,
                                              Attribute = (RouteAttribute) attributes.First()
                                          };
            // ������������ �����
            foreach (var info in methodsMappings)
            {
                RegisterRoute(String.Format("{0}.{1}",info.Controller,info.Action),info.Attribute.Route,new {controller = info.Controller.Replace("Controller",String.Empty), action = info.Action});
            }
        }
    }
}