using digi_sante.Repositories;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace digi_sante
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here  
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IAuthentificationRepository, AuthentificationRepositoryImpl>();
            container.RegisterType<IPatientRepository, PatientRepositoryImpl>();
            container.RegisterType<IDepartementRepository, DepartementRepositoryImpl>();
            container.RegisterType<IAssuranceRepository, AssuranceRepositoryImpl>();
            container.RegisterType<IUserRepository, UserRepositoryImpl>();
            container.RegisterType<IPaiementRepository, PaiementRepositoryImpl>();
            container.RegisterType<IStructureRepository, StructureRepositoryImpl>();
            container.RegisterType<IListeAnalyseRepository, ListeAnalyseRepositoryImpl>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
