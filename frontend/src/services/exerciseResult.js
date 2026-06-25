import axiosInstance from "./api";

export async function addExerciseAttempt(answers, exerciseId){
  const dto = {
    Answers: answers,
    ExerciseId: exerciseId,
  }
  await axiosInstance.post('/exercise-result/add-attempt', dto)
}
export async function getLatestScore(exerciseId)
{
  const response = await axiosInstance.get('/exercise-result/get-latest-attempt-score', 
    {params: {exerciseId: exerciseId}})
  return response.data
}
