import React, { useState, useEffect } from 'react';
import { Navigation, SlidersHorizontal, Plus, Loader2 } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';

const MapComponent = () => {
    const [isLoading, setIsLoading] = useState(true);
    const [userLocation, setUserLocation] = useState(null);
    const [selectedSpot, setSelectedSpot] = useState(null);
    const [showFilters, setShowFilters] = useState(false);

    useEffect(() => {
        // Initialisation de la carte ici
        const initializeMap = async () => {
            try {
                // Simuler le chargement de la carte
                await new Promise(resolve => setTimeout(resolve, 1000));
                setIsLoading(false);
            } catch (error) {
                console.error('Erreur lors de l\'initialisation de la carte:', error);
            }
        };

        initializeMap();
    }, []);

    const handleLocationClick = async () => {
        try {
            // Ici, vous ajouterez la logique pour obtenir la position de l'utilisateur
            console.log('Localisation demandée');
        } catch (error) {
            console.error('Erreur de géolocalisation:', error);
        }
    };

    const handleFilterClick = () => {
        setShowFilters(!showFilters);
    };

    const handleAddSpotClick = () => {
        console.log('Ajouter un spot');
    };

    return (
        <div className="relative w-full h-screen bg-gray-100">
            {/* Zone principale de la carte */}
            <div className="absolute inset-0 bg-blue-50">
                {isLoading ? (
                    <div className="flex items-center justify-center h-full">
                        <Loader2 className="w-8 h-8 animate-spin text-blue-600" />
                    </div>
                ) : (
                    <div className="w-full h-full">
                        {/* La carte sera rendue ici */}
                    </div>
                )}
            </div>

            {/* Contrôles de la carte */}
            <div className="absolute top-4 right-4 space-y-2">
                <button
                    onClick={handleLocationClick}
                    className="p-3 bg-white rounded-full shadow-lg hover:bg-gray-50"
                >
                    <Navigation className="w-6 h-6 text-blue-600" />
                </button>

                <button
                    onClick={handleFilterClick}
                    className="p-3 bg-white rounded-full shadow-lg hover:bg-gray-50"
                >
                    <SlidersHorizontal className="w-6 h-6 text-blue-600" />
                </button>

                <button
                    onClick={handleAddSpotClick}
                    className="p-3 bg-white rounded-full shadow-lg hover:bg-gray-50"
                >
                    <Plus className="w-6 h-6 text-blue-600" />
                </button>
            </div>

            {/* Panneau des filtres */}
            {showFilters && (
                <Card className="absolute top-4 left-4 w-64">
                    <CardContent className="p-4">
                        <h3 className="font-semibold mb-4">Filtres</h3>
                        <div className="space-y-2">
                            <label className="flex items-center space-x-2">
                                <input type="checkbox" className="rounded" />
                                <span>Plongée</span>
                            </label>
                            <label className="flex items-center space-x-2">
                                <input type="checkbox" className="rounded" />
                                <span>Apnée</span>
                            </label>
                            <label className="flex items-center space-x-2">
                                <input type="checkbox" className="rounded" />
                                <span>Randonnée</span>
                            </label>
                        </div>
                    </CardContent>
                </Card>
            )}

            {/* Info spot sélectionné */}
            {selectedSpot && (
                <Card className="absolute bottom-4 left-4 right-4 mx-4">
                    <CardContent className="p-4">
                        <h3 className="font-semibold">{selectedSpot.name}</h3>
                        <p className="text-sm text-gray-600">{selectedSpot.description}</p>
                    </CardContent>
                </Card>
            )}
        </div>
    );
};

export default MapComponent;