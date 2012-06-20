using System;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace FatureJa.Negocio.Infraestrutura
{
    public static class AmbienteDaNuvem
    {
        private static bool _ambienteDisponivel;

        static AmbienteDaNuvem()
        {
            _ambienteDisponivel = VerificarDisponibilidade();
        }

        public static bool AmbienteDisponivel
        {
            get
            {
                if (!_ambienteDisponivel)
                {
                    // verificar novamente, pode ter ficado disponível
                    _ambienteDisponivel = VerificarDisponibilidade();
                }
                return _ambienteDisponivel;
            }
        }

        public static string StringDeConexaoAStorage
        {
            get
            {
                if (AmbienteDisponivel)
                {
                    return RoleEnvironment.GetConfigurationSettingValue("StorageConnectionString");
                }
                else
                {
                    return "UseDevelopmentStorage=true"; // forçar storage local
                }
            }
        }

        private static bool VerificarDisponibilidade()
        {
            try
            {
                return RoleEnvironment.IsAvailable;
            }
            catch (TypeInitializationException)
            {
                return false;
            }
        }
    }
}