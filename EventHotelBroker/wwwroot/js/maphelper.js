/**
 * Map Helper for Google Maps Integration
 */

let map;
let marker;
let autocomplete;

window.mapHelper = {
    initAutocomplete: function (dotNetHelper, inputId, mapId) {
        const input = document.getElementById(inputId);
        if (!input) {
            console.warn("Input element not found: " + inputId + ". Retrying in 500ms...");
            setTimeout(() => this.initAutocomplete(dotNetHelper, inputId, mapId), 500);
            return;
        }

        // Prevent double initialization
        if (input.getAttribute("data-google-initialized")) {
            console.log("Autocomplete already initialized for " + inputId);
            return;
        }

        // Wait for google maps to be available
        if (typeof google === 'undefined' || !google.maps || !google.maps.places) {
            console.log("Google Maps library not ready yet. Retrying in 500ms...");
            setTimeout(() => this.initAutocomplete(dotNetHelper, inputId, mapId), 500);
            return;
        }

        try {
            console.log("Initializing Google Places Autocomplete for " + inputId);

            // Initialize Autocomplete
            autocomplete = new google.maps.places.Autocomplete(input, {
                fields: ["address_components", "geometry", "name", "formatted_address"]
                // types removed for broader search
            });

            input.setAttribute("data-google-initialized", "true");
            input.style.border = "2px solid #28a745"; // Success green border to show JS ran

            autocomplete.addListener("place_changed", () => {
                const place = autocomplete.getPlace();

                if (!place.geometry || !place.geometry.location) {
                    console.error("No details available for input: '" + place.name + "'");
                    return;
                }

                const lat = place.geometry.location.lat();
                const lng = place.geometry.location.lng();
                const address = place.formatted_address;

                // Extract address components
                let city = "";
                let country = "";
                
                if (place.address_components) {
                    for (const component of place.address_components) {
                        const types = component.types;
                        
                        // Broaden city search
                        if (types.includes("locality") || 
                            types.includes("sublocality_level_1") || 
                            types.includes("postal_town") ||
                            types.includes("neighborhood")) {
                            city = component.long_name;
                        }
                        if (types.includes("country")) {
                            country = component.long_name;
                        }
                        if (types.includes("administrative_area_level_1") && !city) {
                            city = component.long_name;
                        }
                    }
                }

                // If city is still empty, use the name of the place
                if (!city) city = place.name;

                console.log("Place Selected:", { address, city, country, lat, lng });

                // Call back to Blazor
                dotNetHelper.invokeMethodAsync("UpdateLocationFromJS", address, city, country, lat, lng);

                // Update Map
                this.showMap(mapId, lat, lng, input);
            });

            // Handle enter key to prevent form submission if the user is selecting from dropdown
            input.addEventListener('keydown', (e) => {
                if (e.key === 'Enter') {
                    e.preventDefault();
                }
            });

        } catch (error) {
            console.error("Error initializing autocomplete:", error);
            setTimeout(() => this.initAutocomplete(dotNetHelper, inputId, mapId), 1000);
        }
    },

    showMap: function (mapId, lat, lng, inputElement) {
        const mapContainer = document.getElementById(mapId);
        if (!mapContainer) {
            console.error("Map container not found: " + mapId);
            return;
        }

        const location = { lat: lat, lng: lng };
        console.log("Showing map at:", location);

        // Make map visible
        mapContainer.style.display = "block";

        // Small delay to ensure container is fully rendered
        setTimeout(() => {
            if (!map) {
                console.log("Creating new map...");
                map = new google.maps.Map(mapContainer, {
                    center: location,
                    zoom: 16,
                    mapTypeControl: false,
                    streetViewControl: false,
                    fullscreenControl: true
                });

                marker = new google.maps.Marker({
                    position: location,
                    map: map,
                    draggable: true,
                    animation: google.maps.Animation.BOUNCE
                });

                console.log("Marker created at:", location);

                // Stop bouncing after 3 seconds
                setTimeout(() => {
                    if (marker) {
                        marker.setAnimation(null);
                        console.log("Marker bouncing stopped");
                    }
                }, 3000);

                // Don't move autocomplete input into map controls - keep it in the form
                // This prevents layout issues

            } else {
                console.log("Updating existing map...");
                map.setCenter(location);
                marker.setPosition(location);
                // Start bouncing animation when marker moves
                marker.setAnimation(google.maps.Animation.BOUNCE);
                setTimeout(() => {
                    if (marker) {
                        marker.setAnimation(null);
                        console.log("Marker bouncing stopped");
                    }
                }, 3000);
            }

            // Fix for grey maps in hidden containers
            google.maps.event.trigger(map, "resize");
            console.log("Map resize triggered");
        }, 100);
    },

    // Show map with existing location data (for edit page)
    showMapWithExistingLocation: function (mapId, lat, lng, dotNetHelper) {
        const mapContainer = document.getElementById(mapId);
        if (!mapContainer) {
            console.error("Map container not found: " + mapId);
            return;
        }

        const location = { lat: lat, lng: lng };
        console.log("Showing map with existing location at:", location);

        // Make map visible
        mapContainer.style.display = "block";

        // Get the address from Blazor and pre-fill the autocomplete input
        if (dotNetHelper) {
            dotNetHelper.invokeMethodAsync("GetHotelAddress").then(function(address) {
                const input = document.getElementById("mapSearchInput");
                if (input && address) {
                    input.value = address;
                    console.log("Autocomplete input pre-filled with:", address);
                }
            });
        }

        // Small delay to ensure container is fully rendered
        setTimeout(() => {
            if (!map) {
                console.log("Creating new map with existing location...");
                map = new google.maps.Map(mapContainer, {
                    center: location,
                    zoom: 16,
                    mapTypeControl: false,
                    streetViewControl: false,
                    fullscreenControl: true
                });

                marker = new google.maps.Marker({
                    position: location,
                    map: map,
                    draggable: true,
                    animation: google.maps.Animation.BOUNCE
                });

                console.log("Marker created at existing location:", location);

                // Stop bouncing after 3 seconds
                setTimeout(() => {
                    if (marker) {
                        marker.setAnimation(null);
                        console.log("Marker bouncing stopped");
                    }
                }, 3000);

            } else {
                console.log("Updating existing map with new location...");
                map.setCenter(location);
                marker.setPosition(location);
                marker.setAnimation(google.maps.Animation.BOUNCE);
                setTimeout(() => {
                    if (marker) {
                        marker.setAnimation(null);
                        console.log("Marker bouncing stopped");
                    }
                }, 3000);
            }

            // Fix for grey maps in hidden containers
            google.maps.event.trigger(map, "resize");
            console.log("Map resize triggered");
        }, 100);
    },

    initHotelMap: function (mapId, lat, lng) {
        const mapContainer = document.getElementById(mapId);
        if (!mapContainer) {
            console.error("Map container not found: " + mapId);
            return;
        }

        const location = { lat: lat, lng: lng };
        console.log("Initializing hotel map at:", location);

        // Wait for google maps to be available
        if (typeof google === 'undefined' || !google.maps) {
            console.log("Google Maps library not ready yet. Retrying in 500ms...");
            setTimeout(() => this.initHotelMap(mapId, lat, lng), 500);
            return;
        }

        try {
            const hotelMap = new google.maps.Map(mapContainer, {
                center: location,
                zoom: 16,
                mapTypeControl: false,
                streetViewControl: false,
                fullscreenControl: true
            });

            const hotelMarker = new google.maps.Marker({
                position: location,
                map: hotelMap,
                animation: google.maps.Animation.BOUNCE,
                title: "Hotel Location"
            });

            console.log("Hotel map and bouncing marker initialized.");

            // Fix for hidden container rendering issues
            setTimeout(() => {
                google.maps.event.trigger(hotelMap, "resize");
                hotelMap.setCenter(location);
            }, 200);

        } catch (error) {
            console.error("Error initializing hotel map:", error);
        }
    }
};
