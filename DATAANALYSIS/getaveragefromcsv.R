#please adjust the setwd according to where this file is located before running the script.
setwd("C:/Users/Du Pyo Park/Documents/COSC 341/Assignment2")


library(repr)
library(dplyr)
data1 <- read_csv("data_log1.csv")
data2 <- read_csv("data_log2.csv")
data3 <- read_csv("data_log2.csv")
data = bind_rows(data1,data2,data3)


grouped_data = data |> filter(Correct == TRUE) |> group_by(Width,Amplitude,Technique)  |> summarise(average = mean(Time))

grouped_data = grouped_data |> mutate(average*(1/1000)) |> rename(average_in_ms = "average * (1/1000)") |> select(-average)

grouped_data = grouped_data |> mutate(fitts_id = log2(Amplitude/Width + 1))

print(grouped_data)

write.csv(grouped_data, 'grouped_data.csv', row.names = FALSE)