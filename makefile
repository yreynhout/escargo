SHELL = /bin/sh

all:
	# Clean distribution
	rm -rf .dist
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6LfaGYAaAAAAAA3_y2HpJrpuEPFIz9ogRqfLo2ZZ --api "http://localhost:8081"
	# Generate tailwind stylesheet
	NODE_ENV=production npx tailwindcss-cli build -o .dist/css/tailwind.css
ci:
	# Copy assets
	cp -r -p static .dist
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6LfaGYAaAAAAAA3_y2HpJrpuEPFIz9ogRqfLo2ZZ --api "http://localhost:8081"
	# Generate tailwind stylesheet
	NODE_ENV=production npx tailwindcss-cli build -o .dist/css/tailwind.css
	# Release to AWS S3 bucket
	# aws s3 sync --delete --acl public-read .dist/ s3://knowledgecrunchers.io
serve:
	# Clean distribution
	rm -rf .dist
	# Copy assets
	cp -r -p static .dist
	# Generate tailwind stylesheet
	npx tailwindcss-cli build -o .dist/css/tailwind.css
	# Render html documents
	dotnet run -c Release -p shell/Shell.fsproj -- --output=.dist --domain=knowledgecrunchers.io --googlerecaptcha=6LfaGYAaAAAAAA3_y2HpJrpuEPFIz9ogRqfLo2ZZ --api "http://localhost:8081"
	# Start watching, rendering and serving html documents
	make -j 2 dotnet-serve dotnet-watch
dotnet-serve:
	dotnet serve -p 8080 -o -d .dist
dotnet-watch:
	dotnet watch run -c Release -p shell/Shell.fsproj -- --output=../.dist --domain=knowledgecrunchers.io --googlerecaptcha=6LfaGYAaAAAAAA3_y2HpJrpuEPFIz9ogRqfLo2ZZ --api "http://localhost:8081"
format:
	# Format F# code
	dotnet fantomas shell/
	# Format F# code
	# dotnet fantomas tail/
  # Format F# code
	dotnet fantomas bone/
  # Format F# code
	dotnet fantomas helix/
	# Format F# code
	dotnet fantomas winkle/