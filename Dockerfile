FROM node:22.19.0-alpine

WORKDIR /app

COPY package*.json ./

RUN apk add --no-cache python3 make g++

RUN npm install

COPY . .

ENV PORT=9000

EXPOSE 9000

CMD ["npm", "start"]
