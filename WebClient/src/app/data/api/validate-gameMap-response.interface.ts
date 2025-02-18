export interface ValidateGameMapResponse{
    success: boolean;
    accessToken: string | null;
    errorCode: string | null;
    errorMessage: string | null;
}