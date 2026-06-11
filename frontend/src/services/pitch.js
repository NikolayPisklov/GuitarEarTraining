import axiosInstance from "./api";

export async function getPitchSamples(){
    const response = await axiosInstance.get("/pitch/getSamples")
    return response
}