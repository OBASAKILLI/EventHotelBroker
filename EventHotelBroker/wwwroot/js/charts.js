// Chart.js Interop for EventHotelBroker

window.chartInstances = {};

window.createChart = function (canvasId, type, data, options) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) {
        console.error(`Canvas element with id '${canvasId}' not found`);
        return false;
    }

    // Destroy existing chart if it exists
    if (window.chartInstances[canvasId]) {
        window.chartInstances[canvasId].destroy();
    }

    // Create new chart
    try {
        window.chartInstances[canvasId] = new Chart(ctx, {
            type: type,
            data: data,
            options: options || {}
        });
        return true;
    } catch (error) {
        console.error('Error creating chart:', error);
        return false;
    }
};

window.updateChart = function (canvasId, newData) {
    const chart = window.chartInstances[canvasId];
    if (!chart) {
        console.error(`Chart with id '${canvasId}' not found`);
        return false;
    }

    try {
        chart.data = newData;
        chart.update();
        return true;
    } catch (error) {
        console.error('Error updating chart:', error);
        return false;
    }
};

window.destroyChart = function (canvasId) {
    const chart = window.chartInstances[canvasId];
    if (chart) {
        chart.destroy();
        delete window.chartInstances[canvasId];
        return true;
    }
    return false;
};

// Predefined chart configurations
window.chartConfigs = {
    // Line chart for revenue/bookings over time
    lineChart: function (labels, datasets) {
        return {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets.map(ds => ({
                    label: ds.label,
                    data: ds.data,
                    borderColor: ds.borderColor || '#667eea',
                    backgroundColor: ds.backgroundColor || 'rgba(102, 126, 234, 0.1)',
                    borderWidth: 3,
                    fill: true,
                    tension: 0.4,
                    pointRadius: 4,
                    pointHoverRadius: 6
                }))
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                        labels: {
                            font: { size: 12, family: "'Segoe UI', sans-serif" },
                            padding: 15,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        titleFont: { size: 14 },
                        bodyFont: { size: 13 },
                        borderColor: '#667eea',
                        borderWidth: 1
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0, 0, 0, 0.05)' },
                        ticks: { font: { size: 11 } }
                    },
                    x: {
                        grid: { display: false },
                        ticks: { font: { size: 11 } }
                    }
                }
            }
        };
    },

    // Bar chart for comparisons
    barChart: function (labels, datasets) {
        return {
            type: 'bar',
            data: {
                labels: labels,
                datasets: datasets.map(ds => ({
                    label: ds.label,
                    data: ds.data,
                    backgroundColor: ds.backgroundColor || [
                        'rgba(102, 126, 234, 0.8)',
                        'rgba(16, 185, 129, 0.8)',
                        'rgba(245, 158, 11, 0.8)',
                        'rgba(239, 68, 68, 0.8)',
                        'rgba(59, 130, 246, 0.8)'
                    ],
                    borderColor: ds.borderColor || [
                        '#667eea',
                        '#10b981',
                        '#f59e0b',
                        '#ef4444',
                        '#3b82f6'
                    ],
                    borderWidth: 2,
                    borderRadius: 8
                }))
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: datasets.length > 1,
                        position: 'top',
                        labels: {
                            font: { size: 12, family: "'Segoe UI', sans-serif" },
                            padding: 15,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        titleFont: { size: 14 },
                        bodyFont: { size: 13 }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: 'rgba(0, 0, 0, 0.05)' },
                        ticks: { font: { size: 11 } }
                    },
                    x: {
                        grid: { display: false },
                        ticks: { font: { size: 11 } }
                    }
                }
            }
        };
    },

    // Doughnut chart for distributions
    doughnutChart: function (labels, data, backgroundColors) {
        return {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: backgroundColors || [
                        'rgba(102, 126, 234, 0.8)',
                        'rgba(16, 185, 129, 0.8)',
                        'rgba(245, 158, 11, 0.8)',
                        'rgba(239, 68, 68, 0.8)',
                        'rgba(59, 130, 246, 0.8)'
                    ],
                    borderColor: '#ffffff',
                    borderWidth: 3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            font: { size: 12, family: "'Segoe UI', sans-serif" },
                            padding: 15,
                            usePointStyle: true
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        padding: 12,
                        titleFont: { size: 14 },
                        bodyFont: { size: 13 }
                    }
                }
            }
        };
    }
};
