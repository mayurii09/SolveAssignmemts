import * as signalR from '@microsoft/signalr';

const API_URL = "http://localhost:5016";

class SignalRService {
    constructor() {
        this.connection = null;
    }

    // Initialize the SignalR connection
    async startConnection() {
        try {
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl(`${API_URL}/stockHub`)
                .build();

            this.connection.onclose(() => this.startConnection());

            await this.connection.start();
            console.log('SignalR Connected successfully');
        } catch (err) {
            console.error('Error while starting SignalR connection: ', err);
            setTimeout(() => this.startConnection(), 5000);
        }
    }

    stopConnection() {
        if (this.connection) {
            this.connection.stop().then(() => {
                console.log('SignalR Disconnected');
            }).catch(err => console.error('Error while stopping SignalR connection: ', err));
        }
    }

    onLowStockAlert(handler) {
        if (this.connection) {
            this.connection.on('ReceiveLowStockAlert', handler);
        }
    }

    onStockUpdate(handler) {
        if (this.connection) {
            this.connection.on('ReceiveStockUpdate', handler);
        }
    }

    //offStockUpdate(handler) {
    //    if (this.connection) {
    //        this.connection.off('ReceiveStockUpdate', handler);
    //    }
    //}

    //offLowStockAlert(handler) {
    //    if (this.connection) {
    //        this.connection.off('ReceiveLowStockAlert', handler);
    //    }
    //}

    sendMessage(message) {
        if (this.connection) {
            this.connection.invoke('SendMessage', message).catch(err => console.error('Error sending message:', err));
        }
    }
}

// Create an instance of the SignalR service to use in components
export const signalRService = new SignalRService();

// Functions to start and stop the connection, as well as subscribe to events
export const startSignalRConnection = async () => {
    await signalRService.startConnection();
};

export const stopSignalRConnection = () => {
    signalRService.stopConnection();
};

export const subscribeToLowStockAlert = (handler) => {
    signalRService.onLowStockAlert(handler);
};

export const subscribeToStockUpdate = (handler) => {
    signalRService.onStockUpdate(handler);
};
