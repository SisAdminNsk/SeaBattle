export interface BasePlayerResponse<T = any> {
    MessageType: string;
    Response: T;
}