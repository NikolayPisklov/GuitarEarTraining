import axiosInstance from "./api";

export async function getPitchSamples(){
    const response = await axiosInstance.get("/pitch/getSamples")
    return response.data
}
export async function getAnswerOptionsForPitch(){
    const response = await axiosInstance.get("/pitch/getAnswerOptions")
    return response.data
}