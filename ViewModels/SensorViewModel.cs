using System;
using System.Linq;                         // 👈 IMPORTANTE para FirstOrDefault
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;

namespace FinanzasApp.ViewModels
{
    public class SensorViewModel : BaseViewModel
    {
        private string locationText = "Pulsa el botón para obtener la ubicación.";
        public string LocationText
        {
            get => locationText;
            set => SetProperty(ref locationText, value);
        }

        private string addressText = "Sin dirección todavía.";
        public string AddressText
        {
            get => addressText;
            set => SetProperty(ref addressText, value);
        }

        public ICommand GetLocationCommand { get; }

        public SensorViewModel()
        {
            Title = "Sensor GPS";
            GetLocationCommand = new Command(async () => await GetLocationAsync());
        }

        private async Task GetLocationAsync()
        {
            try
            {
                // 1) Pedir permisos
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    LocationText = "Permiso de ubicación DENEGADO.";
                    AddressText = "No puedo obtener la dirección sin permisos.";
                    return;
                }

                // 2) Obtener coordenadas
                var request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10));

                var location = await Geolocation.Default.GetLocationAsync(request);

                if (location == null)
                {
                    LocationText = "No se pudo obtener la ubicación.";
                    AddressText = "Sin datos de dirección.";
                    return;
                }

                LocationText = $"Lat: {location.Latitude:F5}, Lon: {location.Longitude:F5}";

                // 3) Geocoding inverso (lat/lon -> país/ciudad/calle)
                var placemarks = await Geocoding.Default.GetPlacemarksAsync(
                    location.Latitude,
                    location.Longitude);

                var place = placemarks?.FirstOrDefault();

                if (place != null)
                {
                    // Armamos una dirección bonita
                    AddressText =
                        $"{place.CountryName}\n" +
                        $"{place.AdminArea} {(!string.IsNullOrWhiteSpace(place.SubAdminArea) ? $"({place.SubAdminArea})" : "")}\n" +
                        $"{place.Locality} {(!string.IsNullOrWhiteSpace(place.SubLocality) ? $"- {place.SubLocality}" : "")}\n" +
                        $"{place.Thoroughfare} {place.SubThoroughfare}";
                }
                else
                {
                    AddressText = "No se pudo determinar la dirección.";
                }
            }
            catch (FeatureNotSupportedException)
            {
                LocationText = "Este dispositivo no soporta GPS.";
                AddressText = "Geocoding no disponible en este dispositivo.";
            }
            catch (FeatureNotEnabledException)
            {
                LocationText = "El GPS está desactivado en el dispositivo.";
                AddressText = "Activa la ubicación para obtener la dirección.";
            }
            catch (PermissionException)
            {
                LocationText = "Permisos de ubicación denegados.";
                AddressText = "Sin permisos no hay dirección.";
            }
            catch (Exception ex)
            {
                LocationText = $"Error: {ex.Message}";
                AddressText = "No se pudo obtener la dirección.";
            }
        }
    }
}
