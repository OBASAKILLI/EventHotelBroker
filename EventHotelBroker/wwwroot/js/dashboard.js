window.renderDashboardCharts = function (revenueLabels, revenueData, paymentLabels, paymentData, categoryLabels, categoryData) {
    // Revenue Line Chart
    const ctxRevenue = document.getElementById('revenueChart');
    if (ctxRevenue) {
        if (window.revenueChartInstance) {
            window.revenueChartInstance.destroy();
        }
        window.revenueChartInstance = new Chart(ctxRevenue, {
            type: 'line',
            data: {
                labels: revenueLabels,
                datasets: [{
                    label: 'Revenue',
                    data: revenueData,
                    borderColor: '#DD7A28', // brand color
                    backgroundColor: 'transparent',
                    borderWidth: 2,
                    pointBackgroundColor: '#DD7A28',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    tension: 0.4 // smooth curve
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#fff',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#e5e7eb',
                        borderWidth: 1,
                        padding: 10,
                        displayColors: false,
                        callbacks: {
                            label: function(context) {
                                return 'KES ' + context.parsed.y.toLocaleString();
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return (value / 1000) + 'k';
                            },
                            color: '#9ca3af',
                            font: {
                                size: 10
                            }
                        },
                        grid: {
                            borderDash: [4, 4],
                            color: '#f3f4f6'
                        },
                        border: {
                            display: false
                        }
                    },
                    x: {
                        ticks: {
                            color: '#9ca3af',
                            font: {
                                size: 10
                            }
                        },
                        grid: {
                            display: false
                        },
                        border: {
                            display: false
                        }
                    }
                }
            }
        });
    }

    // Payment Mix Doughnut Chart
    const ctxPayment = document.getElementById('paymentMixChart');
    if (ctxPayment) {
        if (window.paymentChartInstance) {
            window.paymentChartInstance.destroy();
        }
        window.paymentChartInstance = new Chart(ctxPayment, {
            type: 'doughnut',
            data: {
                labels: paymentLabels,
                datasets: [{
                    data: paymentData,
                    backgroundColor: [
                        '#22c55e', // green-500
                        '#1e3a8a', // blue-900
                        '#3b82f6', // blue-500
                        '#ef4444', // red-500
                        '#DD7A28', // brand
                        '#8b5cf6', // purple-500
                        '#eab308'  // yellow-500
                    ],
                    borderWidth: 2,
                    borderColor: '#ffffff',
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '75%',
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#fff',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#e5e7eb',
                        borderWidth: 1,
                        padding: 10,
                        callbacks: {
                            label: function(context) {
                                return context.label + ': KES ' + context.parsed.toLocaleString();
                            }
                        }
                    }
                }
            }
        });
    }

    // Category Sales Bar Chart
    const ctxCategory = document.getElementById('categoryChart');
    if (ctxCategory && categoryLabels && categoryData) {
        if (window.categoryChartInstance) {
            window.categoryChartInstance.destroy();
        }
        window.categoryChartInstance = new Chart(ctxCategory, {
            type: 'bar',
            data: {
                labels: categoryLabels,
                datasets: [{
                    label: 'Sales',
                    data: categoryData,
                    backgroundColor: '#1e3a8a', // blue-900
                    borderRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        backgroundColor: '#fff',
                        titleColor: '#000',
                        bodyColor: '#000',
                        borderColor: '#e5e7eb',
                        borderWidth: 1,
                        padding: 10,
                        displayColors: false,
                        callbacks: {
                            label: function(context) {
                                return 'KES ' + context.parsed.y.toLocaleString();
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return (value / 1000) + 'k';
                            },
                            color: '#9ca3af',
                            font: {
                                size: 10
                            }
                        },
                        grid: {
                            borderDash: [4, 4],
                            color: '#f3f4f6'
                        },
                        border: {
                            display: false
                        }
                    },
                    x: {
                        ticks: {
                            color: '#9ca3af',
                            font: {
                                size: 10
                            }
                        },
                        grid: {
                            display: false
                        },
                        border: {
                            display: false
                        }
                    }
                }
            }
        });
    }
};
