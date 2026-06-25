import axiosInstance from "./api";
import { useCsrfStore } from "../plugins/piniaStorage";

export async function getCsrfToken() {
    const response = await axiosInstance.get("/csrf-token");

    const csrfStore = useCsrfStore();
    csrfStore.token = response.data.token;
}